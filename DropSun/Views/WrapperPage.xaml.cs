using DropSun.Model.Geolocation;
using DropSun.Model.ViewModels;
using DropSun.Model.Weather;
using DropSun.Views.Application;
using DropSun.Views.Controls;
using Microsoft.UI.Composition;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.ViewManagement;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DropSun.Views
{
    /// <summary>
    /// Responsible for the split view with the sidebar and holding the sidebar items.
    /// </summary>
    public sealed partial class WrapperPage : Page
    {
        private bool isSideBarExpanded = true;
        Storyboard currentAnimation = null;
        Type _currentlyShownPage;

        string debugLocationsJSON;
        string debugForecastsJSON;

        public WrapperPage()
        {
            this.InitializeComponent();
            this.Loaded += WrapperPage_Loaded;
            this.PointerMoved += WrapperPage_PointerMoved;
            this.PointerReleased += WrapperPage_PointerReleased;
            this.DoubleTapped += WrapperPage_DoubleTapped;
            this.PointerExited += WrapperPage_PointerExited;
        }

        private void WrapperPage_Loaded(object sender, RoutedEventArgs e)
        {
            ContentFrame.CornerRadius = new CornerRadius(8, 8, 3, 8);
            FrameNavigationOptions navOptions = new FrameNavigationOptions();
            navOptions.TransitionInfoOverride = new DrillInNavigationTransitionInfo();
            Type pageType = typeof(WeatherView);
            ContentFrame.NavigateToType(pageType, null, navOptions);
            _currentlyShownPage = typeof(WeatherView);
        }

        public async void addLocation(InternalGeolocation SelectedLocation, bool useAnimation)
        {
            SidebarWeatherItem weatherItem = new()
            {
                Location = SelectedLocation,
                Temperature = 0,
                Precipitation = 0,
            };

            Frame frame = new Frame();
            frame.Content = weatherItem;
            frame.Tag = SelectedLocation;

            frame.PointerPressed += Frame_PointerPressed;

            var weatherForecast = await OpenMeteoAPI.GetWeatherAsync(SelectedLocation.latitude, SelectedLocation.longitude);
            weatherItem.Weather = weatherForecast;
            weatherItem.Temperature = (double)weatherForecast.Current.Temperature2M;
            weatherItem.Precipitation = (int)weatherForecast.Current.Precipitation;

            if (!useAnimation) LocationsStackPanel.Children.Add(frame);
            else animateItem(frame);
        }

        public void showSettingsPage()
        {
            if (_currentlyShownPage != typeof(SettingsPage))
            {
                FrameNavigationOptions navOptions = new FrameNavigationOptions();
                navOptions.TransitionInfoOverride = new DrillInNavigationTransitionInfo();
                Type pageType = typeof(SettingsPage);
                ContentFrame.NavigateToType(pageType, null, navOptions);
                _currentlyShownPage = typeof(SettingsPage);
            }
        }

        private bool isAttemptingToDrag = false;
        private bool canDrag = false;
        private Vector2 originalPosition;
        private Vector2 offset;
        private Frame draggingElement;

        private int originalIndex; // starting position of the dragged item
        private int targetIndex;   // target drop position

        private int lastHoverPosition;
        private int dragIndex = 0;

        private async void Frame_PointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (_currentlyShownPage != typeof(WeatherView))
            {
                FrameNavigationOptions navOptions = new FrameNavigationOptions();
                navOptions.TransitionInfoOverride = new DrillInNavigationTransitionInfo();
                Type pageType = typeof(WeatherView);
                ContentFrame.NavigateToType(pageType, null, navOptions);
                _currentlyShownPage = typeof(WeatherView);
            }

            dragIndex++;
            isAttemptingToDrag = true;
            var currentDragIndex = dragIndex;

            var frame = sender as Frame;
            var timeDelay = TimeSpan.FromSeconds(0.4);
            frame.RenderTransform = new CompositeTransform();
            var currentScrollOffset = LocationsScrollViewer.VerticalOffset;

            var transform = frame.TransformToVisual(null) as GeneralTransform;
            var elementPos = transform.TransformPoint(new Windows.Foundation.Point(0, 0));
            originalPosition = new Vector2((float)elementPos.X, (float)elementPos.Y);
            var pointerPos = e.GetCurrentPoint(null).Position;
            offset = new Vector2((float)(pointerPos.X - elementPos.X), (float)(pointerPos.Y - elementPos.Y));

            draggingElement = frame;

            animateScaleOfFrame(frame, 0.75, timeDelay);
            await Task.Delay(timeDelay);
            if (isAttemptingToDrag && draggingElement == frame && dragIndex == currentDragIndex && LocationsScrollViewer.VerticalOffset == currentScrollOffset)
            {
                LocationsScrollViewer.VerticalScrollMode = ScrollingScrollMode.Disabled; // disable scrolling while moving items around
                animateScaleOfFrame(frame, 1.2, TimeSpan.FromSeconds(0.2));
                originalIndex = LocationsStackPanel.Children.IndexOf(frame);
                lastHoverPosition = originalIndex;
                canDrag = true;
            }
            else
            {
                dragIndex++;
                isAttemptingToDrag = false;
                isAttemptingToDrag = false;
                canDrag = false;
                animateScaleOfFrame(draggingElement, 1, TimeSpan.FromSeconds(0.2), 0.75);
                draggingElement = null;
            }
        }

        private void WrapperPage_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            dropItemFromRearranging();
        }

        private void WrapperPage_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            dropItemFromRearranging();
        }

        private async void dropItemFromRearranging()
        {
            LocationsScrollViewer.VerticalScrollMode = ScrollingScrollMode.Auto;
            if (canDrag && draggingElement != null)
            {
                dragIndex++;
                canDrag = false;
                isAttemptingToDrag = false;

                var frame = draggingElement;
                var timeDelay = TimeSpan.FromMilliseconds(2000);

                CompositeTransform renderTransform = (CompositeTransform)frame.RenderTransform;
                var offsetX = renderTransform.TranslateX;
                var offsetY = renderTransform.TranslateY;
                var unitsTravelledDownwards = targetIndex - originalIndex;
                offsetY = renderTransform.TranslateY - unitsTravelledDownwards * frame.ActualHeight;

                //animateScaleOfFrame(frame, 1, timeDelay, frame.Scale.X);

                //draggingElement.ReleasePointerCapture(e.Pointer);
                draggingElement = null;

                LocationsStackPanel.Children.Move((uint)originalIndex, (uint)targetIndex);

                foreach (Frame otherFrame in LocationsStackPanel.Children)
                {
                    otherFrame.RenderTransform = null;
                    otherFrame.Translation = new Vector3(0, 0, 0);
                }

                // animation
                UISettings settings = new();
                Duration duration = new Duration(TimeSpan.FromMilliseconds(500));
                if (settings.AnimationsEnabled)
                {
                    TransformGroup transformGroup = new TransformGroup();
                    ScaleTransform scaleTransform = new ScaleTransform();
                    TranslateTransform translateTransform = new TranslateTransform();
                    transformGroup.Children.Add(scaleTransform);
                    transformGroup.Children.Add(translateTransform);
                    frame.RenderTransform = transformGroup;

                    DoubleAnimationUsingKeyFrames scaleXAnimation = new DoubleAnimationUsingKeyFrames { EnableDependentAnimation = true };
                    DoubleAnimationUsingKeyFrames scaleYAnimation = new DoubleAnimationUsingKeyFrames { EnableDependentAnimation = true };
                    DoubleAnimationUsingKeyFrames translateXAnimation = new DoubleAnimationUsingKeyFrames { EnableDependentAnimation = true };
                    DoubleAnimationUsingKeyFrames translateYAnimation = new DoubleAnimationUsingKeyFrames { EnableDependentAnimation = true };

                    scaleXAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0), Value = 1.2 });
                    scaleXAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = duration.TimeSpan * 0.4, Value = 0.85, EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut } });
                    scaleXAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = duration.TimeSpan, Value = 1, EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut } });

                    scaleYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0), Value = 1.2 });
                    scaleYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = duration.TimeSpan * 0.4, Value = 0.85, EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut } });
                    scaleYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = duration.TimeSpan, Value = 1, EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut } });

                    translateXAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0), Value = offsetX });
                    translateXAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = duration.TimeSpan * 0.5, Value = 0, EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 0.2 } });

                    translateYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0), Value = offsetY });
                    translateYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = duration.TimeSpan * 0.5, Value = 0, EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 0.2 } });

                    Storyboard sb = new Storyboard { Duration = duration };
                    sb.Children.Add(scaleXAnimation);
                    sb.Children.Add(scaleYAnimation);
                    sb.Children.Add(translateXAnimation);
                    sb.Children.Add(translateYAnimation);

                    Storyboard.SetTarget(scaleXAnimation, scaleTransform);
                    Storyboard.SetTargetProperty(scaleXAnimation, "ScaleX");

                    Storyboard.SetTarget(scaleYAnimation, scaleTransform);
                    Storyboard.SetTargetProperty(scaleYAnimation, "ScaleY");

                    Storyboard.SetTarget(translateXAnimation, translateTransform);
                    Storyboard.SetTargetProperty(translateXAnimation, "X");

                    Storyboard.SetTarget(translateYAnimation, translateTransform);
                    Storyboard.SetTargetProperty(translateYAnimation, "Y");

                    sb.Begin();
                }

                // ripple effect
                if (!AppSettings.Instance.DisableSidebarRippleEffect && settings.AnimationsEnabled)
                {
                    await Task.Delay(duration.TimeSpan * 0.25);
                    var rippleOrigin = targetIndex;
                    for (int i = 1; i < LocationsStackPanel.Children.Count; i++)
                    {
                        var upperIndex = rippleOrigin - i; // index is lower, aka items at the top
                        var lowerIndex = rippleOrigin + i; // index is higher, aka items at the bottom

                        // ripple upwards
                        if (upperIndex >= 0) rippleOtherItems(LocationsStackPanel.Children[upperIndex] as Frame, (rippleOrigin - upperIndex) * 3, true);
                        // ripple downwards
                        if (lowerIndex < LocationsStackPanel.Children.Count) rippleOtherItems(LocationsStackPanel.Children[lowerIndex] as Frame, (lowerIndex - rippleOrigin) * 3, false);
                        await Task.Delay(200);
                    }
                }
            }
            else if (isAttemptingToDrag && draggingElement != null)
            {
                dragIndex++;
                isAttemptingToDrag = false;
                isAttemptingToDrag = false;
                canDrag = false;
                animateScaleOfFrame(draggingElement, 1, TimeSpan.FromSeconds(0.2), 0.9);
                draggingElement = null;

            }
        }

        private void WrapperPage_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (isAttemptingToDrag && draggingElement != null)
            {
                dragIndex++;
                isAttemptingToDrag = false;
                isAttemptingToDrag = false;
                canDrag = false;
                animateScaleOfFrame(draggingElement, 1, TimeSpan.FromSeconds(0.2), 0.9);
                draggingElement = null;
            }
        }


        private void animateScaleOfFrame(Frame element, double scale, TimeSpan timeSpan, double from = 1)
        {
            if (element != null)
            {
                if (element.RenderTransform == null) element.RenderTransform = new CompositeTransform();
                if (element.RenderTransform is CompositeTransform transform)
                {
                    UISettings settings = new();
                    if (settings.AnimationsEnabled)
                    {
                        var scaleXAnim = new DoubleAnimation
                        {
                            From = from,
                            To = scale,
                            Duration = timeSpan,
                            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                        };

                        var scaleYAnim = new DoubleAnimation
                        {
                            From = from,
                            To = scale,
                            Duration = timeSpan,
                            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                        };

                        var storyboard = new Storyboard();
                        storyboard.Children.Add(scaleXAnim);
                        storyboard.Children.Add(scaleYAnim);

                        Storyboard.SetTarget(scaleXAnim, element);
                        Storyboard.SetTargetProperty(scaleXAnim, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");

                        Storyboard.SetTarget(scaleYAnim, element);
                        Storyboard.SetTargetProperty(scaleYAnim, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");

                        storyboard.Begin();
                    }
                }
            }
        }

        private void WrapperPage_PointerMoved(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (canDrag && draggingElement != null)
            {
                var pointerPos = e.GetCurrentPoint(null).Position;

                double newX = pointerPos.X - offset.X;
                double newY = pointerPos.Y - offset.Y;

                // make it so the item can't be dragged sideways very far without a force pushing it back, symbolizing the user is mainly meant to drag it up and down.
                double horizontalOffset = newX - originalPosition.X;
                var pullBackStrength = 3;
                double correctedX = originalPosition.X + horizontalOffset * (1 / Math.Sqrt(Math.Abs(horizontalOffset) * pullBackStrength));

                if (draggingElement.RenderTransform is CompositeTransform transform)
                {
                    transform.TranslateX = correctedX - originalPosition.X;
                    transform.TranslateY = newY - originalPosition.Y;
                }

                // calculate where to put the item
                var relativePointerPos = e.GetCurrentPoint(LocationsStackPanel).Position;

                double rnewX = relativePointerPos.X - offset.X;
                double rnewY = relativePointerPos.Y - offset.Y;

                // Calculate where the item would be dropped
                targetIndex = (int)Math.Round(rnewY / draggingElement.ActualHeight);

                targetIndex = Math.Clamp(targetIndex, 0, LocationsStackPanel.Children.Count - 1);

                // Update item shifting
                if (targetIndex != lastHoverPosition)
                {
                    lastHoverPosition = targetIndex;
                    UpdateItemPositions();
                }
            }
            else if (isAttemptingToDrag && draggingElement != null)
            {
                // before the item is being dragged, it can still subtly be pulled towards the cursor to indicate that dragging is about to occur
                var pointerPos = e.GetCurrentPoint(null).Position;

                double newX = pointerPos.X - offset.X;
                double newY = pointerPos.Y - offset.Y;

                double horizontalOffset = newX - originalPosition.X;
                double verticalOffset = newY - originalPosition.Y;
                var horizontalPullBackStrength = 3;
                var verticalPullBackStrength = 0.25;
                double correctedX = originalPosition.X + horizontalOffset * (1 / Math.Sqrt(Math.Abs(horizontalOffset) * horizontalPullBackStrength));
                double correctedY = originalPosition.Y + verticalOffset * (1 / Math.Sqrt(Math.Abs(verticalOffset) * verticalPullBackStrength));

                if (draggingElement.RenderTransform is CompositeTransform transform)
                {
                    transform.TranslateX = correctedX - originalPosition.X;
                    transform.TranslateY = correctedY - originalPosition.Y;
                }
            }
        }

        private void UpdateItemPositions()
        {
            for (int i = 0; i < LocationsStackPanel.Children.Count; i++)
            {
                var item = LocationsStackPanel.Children[i];

                if (item == draggingElement) continue;

                if (i >= targetIndex && i < originalIndex)
                {
                    //item.Translation = new Vector3(0, item.ActualSize.Y, 0);
                    AnimateTranslateY(item, item.ActualSize.Y);
                }
                else if (i <= targetIndex && i > originalIndex)
                {
                    //item.Translation = new Vector3(0, -item.ActualSize.Y, 0);
                    AnimateTranslateY(item, -item.ActualSize.Y);
                }
                else
                {
                    //item.Translation = new Vector3(0, 0, 0);
                    AnimateTranslateY(item, 0);
                }
            }
        }

        private void AnimateTranslateY(UIElement element, double toY, int duration = 200)
        {
            // get current transform state without resetting
            var translateTransform = element.RenderTransform as TranslateTransform
                ?? (element.RenderTransform as TransformGroup)?.Children
                    .OfType<TranslateTransform>()
                    .FirstOrDefault();

            // preserve existing transforms if present
            if (translateTransform == null)
            {
                translateTransform = new TranslateTransform();
                element.RenderTransform = element.RenderTransform switch
                {
                    TransformGroup existingGroup => new TransformGroup()
                    {
                        Children = new TransformCollection() { translateTransform }
                    },
                    { } existingTransform => new TransformGroup()
                    {
                        Children = { existingTransform, translateTransform }
                    },
                    _ => translateTransform
                };
            }

            var currentY = translateTransform.Y;

            if (toY != currentY)
            {
                UISettings settings = new();
                if (settings.AnimationsEnabled)
                {
                    var animation = new DoubleAnimation
                    {
                        EasingFunction = new ExponentialEase() { Exponent = 3, EasingMode = EasingMode.EaseOut },
                        From = currentY,
                        To = toY,
                        Duration = TimeSpan.FromMilliseconds(duration),
                        EnableDependentAnimation = true
                    };

                    Storyboard.SetTarget(animation, translateTransform);
                    Storyboard.SetTargetProperty(animation, "Y");

                    var storyboard = new Storyboard();
                    storyboard.Children.Add(animation);

                    storyboard.Begin();
                }
                else
                {
                    element.Translation = new Vector3(0, (float)toY, 0);
                }
            }
        }


        private void animateItem(Frame frame)
        {
            frame.Opacity = 1;
            LocationsStackPanel.Children.Add(frame);

            var uiSettings = new UISettings();
            if (uiSettings.AnimationsEnabled) animateAddedItem(frame);
        }

        private async void animateAddedItem(Frame frame)
        {
            frame.RenderTransformOrigin = new Point(0.5, 0.5);
            TransformGroup transformGroup = new TransformGroup();
            ScaleTransform scaleTransform = new ScaleTransform();
            TranslateTransform translateTransform = new TranslateTransform();
            transformGroup.Children.Add(scaleTransform);
            transformGroup.Children.Add(translateTransform);
            frame.RenderTransform = transformGroup;

            Duration duration = new Duration(TimeSpan.FromSeconds(1));

            DoubleAnimationUsingKeyFrames scaleXAnimation = new DoubleAnimationUsingKeyFrames { EnableDependentAnimation = true };
            DoubleAnimationUsingKeyFrames scaleYAnimation = new DoubleAnimationUsingKeyFrames { EnableDependentAnimation = true };
            DoubleAnimationUsingKeyFrames opacityAnimation = new DoubleAnimationUsingKeyFrames { EnableDependentAnimation = true };
            DoubleAnimationUsingKeyFrames translateYAnimation = new DoubleAnimationUsingKeyFrames { EnableDependentAnimation = true };

            scaleXAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0), Value = 10 });
            scaleXAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0.4), Value = 0.7, EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut } });
            scaleXAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(1), Value = 1, EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 0.3 } });

            scaleYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0), Value = 10 });
            scaleYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0.4), Value = 0.7, EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut } });
            scaleYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(1), Value = 1, EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 0.3 } });

            opacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0), Value = 0 });
            opacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0.3), Value = 1, EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut } });

            translateYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0), Value = 500 });
            translateYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0.35), Value = -5, EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut } });
            translateYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0.9), Value = 0, EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 0.5 } });

            Storyboard sb = new Storyboard { Duration = duration };
            sb.Children.Add(scaleXAnimation);
            sb.Children.Add(scaleYAnimation);
            sb.Children.Add(opacityAnimation);
            sb.Children.Add(translateYAnimation);

            Storyboard.SetTarget(scaleXAnimation, scaleTransform);
            Storyboard.SetTargetProperty(scaleXAnimation, "ScaleX");

            Storyboard.SetTarget(scaleYAnimation, scaleTransform);
            Storyboard.SetTargetProperty(scaleYAnimation, "ScaleY");

            Storyboard.SetTarget(opacityAnimation, frame);
            Storyboard.SetTargetProperty(opacityAnimation, "Opacity");

            Storyboard.SetTarget(translateYAnimation, translateTransform);
            Storyboard.SetTargetProperty(translateYAnimation, "Y");

            sb.Begin();

            List<UIElement> listOfFrames = LocationsStackPanel.Children
                .Take(LocationsStackPanel.Children.Count - 1)
                .Reverse()
                .ToList();

            if (!AppSettings.Instance.DisableSidebarRippleEffect)
            {
                await Task.Delay(300);

                int i = 1;
                foreach (Frame otherFrame in listOfFrames)
                {
                    rippleOtherItems(otherFrame, i * 2);
                    await Task.Delay(200);
                    i++;
                }
            }
        }

        private void rippleOtherItems(Frame frame, int index, bool rippleUpwards = true)
        {
            var modifier = rippleUpwards ? 1 : -1;
            frame.RenderTransformOrigin = new Point(0.5, 0.5);
            TransformGroup transformGroup = new TransformGroup();
            ScaleTransform scaleTransform = new ScaleTransform();
            TranslateTransform translateTransform = new TranslateTransform();
            transformGroup.Children.Add(scaleTransform);
            transformGroup.Children.Add(translateTransform);
            frame.RenderTransform = transformGroup;

            Duration duration = new Duration(TimeSpan.FromSeconds(1.3));

            DoubleAnimationUsingKeyFrames scaleXAnimation = new DoubleAnimationUsingKeyFrames { EnableDependentAnimation = true };
            DoubleAnimationUsingKeyFrames scaleYAnimation = new DoubleAnimationUsingKeyFrames { EnableDependentAnimation = true };
            DoubleAnimationUsingKeyFrames translateYAnimation = new DoubleAnimationUsingKeyFrames { EnableDependentAnimation = true };

            scaleXAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0), Value = 1 });
            scaleXAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0.4), Value = 1 - 0.1/index, EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut } });
            scaleXAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(1.3), Value = 1, EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 0.5 } });

            scaleYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0), Value = 1 });
            scaleYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0.4), Value = 1 - 0.1/index, EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut } });
            scaleYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(1.3), Value = 1, EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 0.5 } });

            translateYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0), Value = 0 });
            translateYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0.35), Value = modifier * (- 3 / index), EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut } });
            translateYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(1.3), Value = 0, EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 0.4 } });

            Storyboard sb = new Storyboard { Duration = duration };
            sb.Children.Add(scaleXAnimation);
            sb.Children.Add(scaleYAnimation);
            sb.Children.Add(translateYAnimation);

            Storyboard.SetTarget(scaleXAnimation, scaleTransform);
            Storyboard.SetTargetProperty(scaleXAnimation, "ScaleX");

            Storyboard.SetTarget(scaleYAnimation, scaleTransform);
            Storyboard.SetTargetProperty(scaleYAnimation, "ScaleY");

            Storyboard.SetTarget(translateYAnimation, translateTransform);
            Storyboard.SetTargetProperty(translateYAnimation, "Y");

            sb.Begin();
        }

        public void toggleSidebarState()
        {
            if (isSideBarExpanded)
            {
                collapseSidebar();
                ContentFrame.CornerRadius = new Microsoft.UI.Xaml.CornerRadius(8, 8, 4, 4);
            }
            else
            {
                expandSidebar();
                ContentFrame.CornerRadius = new Microsoft.UI.Xaml.CornerRadius(8, 8, 4, 8);
            }
        }

        public void expandSidebar()
        {
            isSideBarExpanded = true;
            if (currentAnimation != null)
            {
                currentAnimation.Stop();
            }

            var uiSettings = new UISettings();
            if (uiSettings.AnimationsEnabled)
            {
                Duration duration = new Duration(TimeSpan.FromSeconds(0.3));
                CircleEase circleEase = new CircleEase();
                circleEase.EasingMode = EasingMode.EaseInOut;

                DoubleAnimation doubleAnimation = new DoubleAnimation();
                doubleAnimation.Duration = duration;
                doubleAnimation.From = SidebarContainer.ActualWidth;
                doubleAnimation.To = 349;
                doubleAnimation.EnableDependentAnimation = true;
                doubleAnimation.EasingFunction = circleEase;

                Storyboard sb = new Storyboard();
                sb.Duration = duration;
                sb.Children.Add(doubleAnimation);

                Storyboard.SetTarget(doubleAnimation, SidebarContainer);
                Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("Width").Path);

                currentAnimation = sb;
                sb.Begin();
            }
            else SidebarContainer.Width = 349;
        }

        public void collapseSidebar()
        {
            isSideBarExpanded = false;
            if (currentAnimation != null)
            {
                currentAnimation.Stop();
            }

            var uiSettings = new UISettings();
            if (uiSettings.AnimationsEnabled)
            {
                Duration duration = new Duration(TimeSpan.FromSeconds(0.3));
                var ease = new CircleEase();
                ease.EasingMode = EasingMode.EaseInOut;

                DoubleAnimation doubleAnimation = new DoubleAnimation();
                doubleAnimation.Duration = duration;
                doubleAnimation.From = SidebarContainer.ActualWidth;
                doubleAnimation.To = 24;
                doubleAnimation.EnableDependentAnimation = true;
                doubleAnimation.EasingFunction = ease;

                Storyboard sb = new Storyboard();
                sb.Duration = duration;
                sb.Children.Add(doubleAnimation);

                Storyboard.SetTarget(doubleAnimation, SidebarContainer);
                Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("Width").Path);

                currentAnimation = sb;
                sb.Begin();
            }
            else SidebarContainer.Width = 24;
        }

        private async void SidebarContainer_RightTapped(object sender, Microsoft.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(debugLocationsJSON)) debugLocationsJSON = await ReadDebugLocationsFileAsync();
            if (string.IsNullOrEmpty(debugForecastsJSON)) debugForecastsJSON = await ReadDebugWeatherFileAsync();
            
            Random rnd = new Random();
            var number = rnd.Next(0, 26);

            // get JSON weather
            var location = JsonSerializer.Deserialize<List<InternalGeolocation>>(debugLocationsJSON)[number];
            var weatherForecast = JsonSerializer.Deserialize<List<OpenMeteoWeatherOverview>>(debugForecastsJSON)[number];

            SidebarWeatherItem weatherItem = new()
            {
                Location = location,
                Weather = weatherForecast
            };

            Frame frame = new Frame();
            frame.Content = weatherItem;

            frame.PointerPressed += Frame_PointerPressed;
            
            animateItem(frame);
        }

        public async Task<string> ReadDebugLocationsFileAsync()
        {
            var uri = new Uri("ms-appx:///Assets/DebugSources/debug_locations.json");
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(uri);
            return await FileIO.ReadTextAsync(file);
        }

        public async Task<string> ReadDebugWeatherFileAsync()
        {
            var uri = new Uri("ms-appx:///Assets/DebugSources/weather_debug_locations.json");
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(uri);
            return await FileIO.ReadTextAsync(file);
        }

        private void SidebarContainer_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            LocationsStackPanel.Children.Clear();
        }

        private void saveSidebarState()
        {

        }
    }
}
