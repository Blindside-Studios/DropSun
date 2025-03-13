using DropSun.Model.Geolocation;
using DropSun.Model.Weather;
using DropSun.Views.Controls;
using Microsoft.UI.Composition;
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
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Metadata;
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
            dragIndex++;
            isAttemptingToDrag = true;
            var currentDragIndex = dragIndex;

            var frame = sender as Frame;
            var timeDelay = TimeSpan.FromSeconds(0.8);
            frame.RenderTransform = new CompositeTransform();
            //frame.Translation = new Vector3(frame.Translation.X, frame.Translation.Y, 99999); // ensure it's always rendered on top 

            var transform = frame.TransformToVisual(null) as GeneralTransform;
            var elementPos = transform.TransformPoint(new Windows.Foundation.Point(0, 0));
            originalPosition = new Vector2((float)elementPos.X, (float)elementPos.Y);
            var pointerPos = e.GetCurrentPoint(null).Position;
            offset = new Vector2((float)(pointerPos.X - elementPos.X), (float)(pointerPos.Y - elementPos.Y));

            draggingElement = frame;
                
            animateScaleOfFrame(frame, 0.75, timeDelay);
            await Task.Delay(timeDelay);
            if (isAttemptingToDrag && draggingElement == frame && dragIndex == currentDragIndex)
            {
                animateScaleOfFrame(frame, 1.2, TimeSpan.FromSeconds(0.2));
                originalIndex = LocationsStackPanel.Children.IndexOf(frame);
                lastHoverPosition = originalIndex;
                canDrag = true;
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
                Duration duration = new Duration(TimeSpan.FromMilliseconds(500));

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

                await Task.Delay(duration.TimeSpan * 0.25);

                // ripple effect
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
            if (element.RenderTransform is CompositeTransform transform)
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

        private void WrapperPage_PointerMoved(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (canDrag && draggingElement != null)
            {
                var pointerPos = e.GetCurrentPoint(null).Position;

                double newX = pointerPos.X - offset.X;
                double newY = pointerPos.Y - offset.Y;

                // make it so the item can't be dragged sideways very far without a force pushing it back, symbolizing the user is mainly meant to drag it up and down.
                double horizontalOffset = newX - originalPosition.X;
                var pullBackStrength = 0.1;
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

            await Task.Delay(300);

            int i = 1;
            foreach (Frame otherFrame in listOfFrames)
            {
                rippleOtherItems(otherFrame, i * 2);
                await Task.Delay(200);
                i++;
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

        private void SidebarContainer_RightTapped(object sender, Microsoft.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            Random rnd = new Random();
            var weatherForecast = OpenMeteoAPI.GetWeatherFromJSON(debugJSONLocations[rnd.Next(0, 5)]);
            SidebarWeatherItem weatherItem = new()
            {
                Location = new InternalGeolocation
                {
                    name = $"Location {LocationsStackPanel.Children.Count().ToString()}",
                    country_code = "US",
                    id = 0,
                    latitude = weatherForecast.Latitude,
                    longitude = weatherForecast.Longitude,
                    state_code = "CA"
                }
            };

            Frame frame = new Frame();
            frame.Content = weatherItem;

            frame.PointerPressed += Frame_PointerPressed;

            weatherItem.Weather = weatherForecast;
            
            animateItem(frame);
        }

        private void SidebarContainer_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            LocationsStackPanel.Children.Clear();
        }

        private void saveSidebarState()
        {

        }

        private string[] debugJSONLocations =
        {
            """{"latitude":1.0,"longitude":1.0,"generationtime_ms":2.395033836364746,"utc_offset_seconds":3600,"timezone":"Europe/Berlin","timezone_abbreviation":"GMT+1","elevation":0.0,"current_units":{"time":"iso8601","interval":"seconds","temperature_2m":"°C","relative_humidity_2m":"%","apparent_temperature":"°C","is_day":"","precipitation":"mm","rain":"mm","showers":"mm","snowfall":"cm","weather_code":"wmo code","cloud_cover":"%","pressure_msl":"hPa","surface_pressure":"hPa","wind_speed_10m":"km/h","wind_direction_10m":"°","wind_gusts_10m":"km/h"},"current":{"time":"2025-02-05T21:15","interval":900,"temperature_2m":28.0,"relative_humidity_2m":83,"apparent_temperature":32.5,"is_day":0,"precipitation":0.30,"rain":0.00,"showers":0.30,"snowfall":0.00,"weather_code":95,"cloud_cover":100,"pressure_msl":1010.1,"surface_pressure":1010.1,"wind_speed_10m":14.0,"wind_direction_10m":215,"wind_gusts_10m":23.8},"hourly_units":{"time":"iso8601","temperature_2m":"°C","apparent_temperature":"°C","precipitation_probability":"%","weather_code":"wmo code","visibility":"m","wind_speed_10m":"km/h","is_day":""},"hourly":{"time":["2025-02-05T21:00","2025-02-05T22:00","2025-02-05T23:00","2025-02-06T00:00","2025-02-06T01:00","2025-02-06T02:00"],"temperature_2m":[28.0,27.9,27.9,27.8,27.5,27.4],"apparent_temperature":[32.4,32.7,32.8,32.9,32.6,32.4],"precipitation_probability":[45,43,38,40,45,48],"weather_code":[95,80,80,80,95,95],"visibility":[24140.00,24140.00,24140.00,24140.00,24140.00,14880.00],"wind_speed_10m":[14.5,13.0,11.5,10.9,11.2,10.5],"is_day":[0,0,0,0,0,0]},"daily_units":{"time":"iso8601","weather_code":"wmo code","temperature_2m_max":"°C","temperature_2m_min":"°C","apparent_temperature_max":"°C","apparent_temperature_min":"°C","precipitation_sum":"mm"},"daily":{"time":["2025-02-05","2025-02-06","2025-02-07","2025-02-08","2025-02-09","2025-02-10","2025-02-11"],"weather_code":[96,95,80,95,95,80,80],"temperature_2m_max":[28.6,28.8,28.8,28.6,28.6,29.0,28.6],"temperature_2m_min":[27.2,27.4,27.7,27.8,27.5,28.0,28.0],"apparent_temperature_max":[33.9,34.0,35.3,34.1,34.2,34.3,34.4],"apparent_temperature_min":[31.6,31.8,32.9,31.1,32.1,33.1,33.1],"precipitation_sum":[12.30,3.90,1.40,4.10,11.20,1.40,3.80]}}""",
            """{"latitude":49.9375,"longitude":50.0,"generationtime_ms":1.9748210906982422,"utc_offset_seconds":3600,"timezone":"Europe/Berlin","timezone_abbreviation":"GMT+1","elevation":6.0,"current_units":{"time":"iso8601","interval":"seconds","temperature_2m":"°C","relative_humidity_2m":"%","apparent_temperature":"°C","is_day":"","precipitation":"mm","rain":"mm","showers":"mm","snowfall":"cm","weather_code":"wmo code","cloud_cover":"%","pressure_msl":"hPa","surface_pressure":"hPa","wind_speed_10m":"km/h","wind_direction_10m":"°","wind_gusts_10m":"km/h"},"current":{"time":"2025-02-05T21:15","interval":900,"temperature_2m":-7.4,"relative_humidity_2m":81,"apparent_temperature":-13.0,"is_day":0,"precipitation":0.00,"rain":0.00,"showers":0.00,"snowfall":0.00,"weather_code":3,"cloud_cover":93,"pressure_msl":1032.6,"surface_pressure":1031.8,"wind_speed_10m":15.6,"wind_direction_10m":75,"wind_gusts_10m":24.8},"hourly_units":{"time":"iso8601","temperature_2m":"°C","apparent_temperature":"°C","precipitation_probability":"%","weather_code":"wmo code","visibility":"m","wind_speed_10m":"km/h","is_day":""},"hourly":{"time":["2025-02-05T21:00","2025-02-05T22:00","2025-02-05T23:00","2025-02-06T00:00","2025-02-06T01:00","2025-02-06T02:00"],"temperature_2m":[-7.4,-7.6,-7.8,-8.0,-8.2,-8.2],"apparent_temperature":[-12.9,-13.2,-13.4,-13.5,-13.7,-13.7],"precipitation_probability":[0,0,0,0,0,0],"weather_code":[3,3,2,2,2,2],"visibility":[28360.00,27680.00,25500.00,24040.00,22360.00,22100.00],"wind_speed_10m":[15.6,15.9,15.6,15.5,15.3,15.4],"is_day":[0,0,0,0,0,0]},"daily_units":{"time":"iso8601","weather_code":"wmo code","temperature_2m_max":"°C","temperature_2m_min":"°C","apparent_temperature_max":"°C","apparent_temperature_min":"°C","precipitation_sum":"mm"},"daily":{"time":["2025-02-05","2025-02-06","2025-02-07","2025-02-08","2025-02-09","2025-02-10","2025-02-11"],"weather_code":[3,3,3,0,3,2,77],"temperature_2m_max":[-2.9,-3.3,-2.3,-2.0,-2.7,-1.2,-2.1],"temperature_2m_min":[-7.8,-8.3,-6.6,-7.6,-7.2,-7.1,-6.8],"apparent_temperature_max":[-8.6,-10.0,-8.8,-8.1,-8.6,-6.1,-6.0],"apparent_temperature_min":[-13.4,-14.2,-12.1,-13.2,-12.8,-12.1,-10.8],"precipitation_sum":[0.00,0.00,0.00,0.00,0.00,0.00,0.00]}}""",
            """{"latitude":24.9,"longitude":25.100002,"generationtime_ms":1.3179779052734375,"utc_offset_seconds":3600,"timezone":"Europe/Berlin","timezone_abbreviation":"GMT+1","elevation":473.0,"current_units":{"time":"iso8601","interval":"seconds","temperature_2m":"°C","relative_humidity_2m":"%","apparent_temperature":"°C","is_day":"","precipitation":"mm","rain":"mm","showers":"mm","snowfall":"cm","weather_code":"wmo code","cloud_cover":"%","pressure_msl":"hPa","surface_pressure":"hPa","wind_speed_10m":"km/h","wind_direction_10m":"°","wind_gusts_10m":"km/h"},"current":{"time":"2025-02-05T21:15","interval":900,"temperature_2m":13.9,"relative_humidity_2m":46,"apparent_temperature":10.3,"is_day":0,"precipitation":0.00,"rain":0.00,"showers":0.00,"snowfall":0.00,"weather_code":2,"cloud_cover":78,"pressure_msl":1022.1,"surface_pressure":966.5,"wind_speed_10m":13.4,"wind_direction_10m":31,"wind_gusts_10m":19.1},"hourly_units":{"time":"iso8601","temperature_2m":"°C","apparent_temperature":"°C","precipitation_probability":"%","weather_code":"wmo code","visibility":"m","wind_speed_10m":"km/h","is_day":""},"hourly":{"time":["2025-02-05T21:00","2025-02-05T22:00","2025-02-05T23:00","2025-02-06T00:00","2025-02-06T01:00","2025-02-06T02:00"],"temperature_2m":[14.1,13.5,13.1,12.2,11.6,11.1],"apparent_temperature":[10.4,9.8,8.5,8.1,7.7,7.6],"precipitation_probability":[0,0,0,0,0,0],"weather_code":[3,2,1,1,2,2],"visibility":[24140.00,24140.00,24140.00,24140.00,24140.00,24140.00],"wind_speed_10m":[13.4,12.8,17.4,14.6,13.6,11.3],"is_day":[0,0,0,0,0,0]},"daily_units":{"time":"iso8601","weather_code":"wmo code","temperature_2m_max":"°C","temperature_2m_min":"°C","apparent_temperature_max":"°C","apparent_temperature_min":"°C","precipitation_sum":"mm"},"daily":{"time":["2025-02-05","2025-02-06","2025-02-07","2025-02-08","2025-02-09","2025-02-10","2025-02-11"],"weather_code":[3,3,3,3,3,2,1],"temperature_2m_max":[18.9,17.7,16.1,17.3,17.6,23.1,19.1],"temperature_2m_min":[11.0,10.2,8.8,8.7,9.4,10.6,9.8],"apparent_temperature_max":[15.0,13.5,11.1,12.6,13.5,18.5,14.3],"apparent_temperature_min":[8.1,6.5,3.2,4.1,4.8,6.5,7.3],"precipitation_sum":[0.00,0.00,0.00,0.00,0.00,0.00,0.00]}}""",
            """{"latitude":48.0,"longitude":14.999998,"generationtime_ms":0.5103349685668945,"utc_offset_seconds":3600,"timezone":"Europe/Berlin","timezone_abbreviation":"GMT+1","elevation":402.0,"current_units":{"time":"iso8601","interval":"seconds","temperature_2m":"°C","relative_humidity_2m":"%","apparent_temperature":"°C","is_day":"","precipitation":"mm","rain":"mm","showers":"mm","snowfall":"cm","weather_code":"wmo code","cloud_cover":"%","pressure_msl":"hPa","surface_pressure":"hPa","wind_speed_10m":"km/h","wind_direction_10m":"°","wind_gusts_10m":"km/h"},"current":{"time":"2025-02-05T21:15","interval":900,"temperature_2m":0.6,"relative_humidity_2m":68,"apparent_temperature":-2.9,"is_day":0,"precipitation":0.00,"rain":0.00,"showers":0.00,"snowfall":0.00,"weather_code":0,"cloud_cover":0,"pressure_msl":1041.2,"surface_pressure":990.5,"wind_speed_10m":5.5,"wind_direction_10m":247,"wind_gusts_10m":18.4},"hourly_units":{"time":"iso8601","temperature_2m":"°C","apparent_temperature":"°C","precipitation_probability":"%","weather_code":"wmo code","visibility":"m","wind_speed_10m":"km/h","is_day":""},"hourly":{"time":["2025-02-05T21:00","2025-02-05T22:00","2025-02-05T23:00","2025-02-06T00:00","2025-02-06T01:00","2025-02-06T02:00"],"temperature_2m":[0.7,0.6,0.6,0.5,0.5,0.4],"apparent_temperature":[-2.9,-2.8,-2.8,-2.6,-2.3,-2.2],"precipitation_probability":[0,0,0,0,0,0],"weather_code":[0,0,3,3,3,3],"visibility":[56640.00,56080.00,52240.00,47760.00,46140.00,41960.00],"wind_speed_10m":[5.7,5.1,4.8,2.8,1.8,0.5],"is_day":[0,0,0,0,0,0]},"daily_units":{"time":"iso8601","weather_code":"wmo code","temperature_2m_max":"°C","temperature_2m_min":"°C","apparent_temperature_max":"°C","apparent_temperature_min":"°C","precipitation_sum":"mm"},"daily":{"time":["2025-02-05","2025-02-06","2025-02-07","2025-02-08","2025-02-09","2025-02-10","2025-02-11"],"weather_code":[45,77,3,3,3,3,3],"temperature_2m_max":[2.7,2.9,7.2,1.6,1.0,3.4,5.5],"temperature_2m_min":[-4.3,0.1,-1.0,-2.4,-3.2,-2.8,0.2],"apparent_temperature_max":[-1.4,-1.4,3.2,-3.4,-3.7,-1.8,0.3],"apparent_temperature_min":[-7.6,-3.9,-5.5,-6.9,-7.0,-6.7,-5.3],"precipitation_sum":[0.00,0.00,0.00,0.00,0.00,0.00,0.00]}}""",
            """{"latitude":19.0,"longitude":80.0,"generationtime_ms":3.2535791397094727,"utc_offset_seconds":3600,"timezone":"Europe/Berlin","timezone_abbreviation":"GMT+1","elevation":125.0,"current_units":{"time":"iso8601","interval":"seconds","temperature_2m":"°C","relative_humidity_2m":"%","apparent_temperature":"°C","is_day":"","precipitation":"mm","rain":"mm","showers":"mm","snowfall":"cm","weather_code":"wmo code","cloud_cover":"%","pressure_msl":"hPa","surface_pressure":"hPa","wind_speed_10m":"km/h","wind_direction_10m":"°","wind_gusts_10m":"km/h"},"current":{"time":"2025-02-05T21:15","interval":900,"temperature_2m":22.7,"relative_humidity_2m":66,"apparent_temperature":24.3,"is_day":0,"precipitation":0.00,"rain":0.00,"showers":0.00,"snowfall":0.00,"weather_code":2,"cloud_cover":44,"pressure_msl":1010.0,"surface_pressure":995.5,"wind_speed_10m":2.9,"wind_direction_10m":90,"wind_gusts_10m":6.5},"hourly_units":{"time":"iso8601","temperature_2m":"°C","apparent_temperature":"°C","precipitation_probability":"%","weather_code":"wmo code","visibility":"m","wind_speed_10m":"km/h","is_day":""},"hourly":{"time":["2025-02-05T21:00","2025-02-05T22:00","2025-02-05T23:00","2025-02-06T00:00","2025-02-06T01:00","2025-02-06T02:00"],"temperature_2m":[22.7,22.6,21.2,21.2,21.2,20.8],"apparent_temperature":[24.3,24.0,22.6,22.5,22.6,22.8],"precipitation_probability":[0,0,0,0,0,0],"weather_code":[2,2,1,0,0,0],"visibility":[24140.00,24140.00,24140.00,24140.00,24140.00,24140.00],"wind_speed_10m":[2.9,3.3,5.4,5.9,6.3,2.8],"is_day":[0,0,0,0,0,0]},"daily_units":{"time":"iso8601","weather_code":"wmo code","temperature_2m_max":"°C","temperature_2m_min":"°C","apparent_temperature_max":"°C","apparent_temperature_min":"°C","precipitation_sum":"mm"},"daily":{"time":["2025-02-05","2025-02-06","2025-02-07","2025-02-08","2025-02-09","2025-02-10","2025-02-11"],"weather_code":[3,1,2,2,1,1,2],"temperature_2m_max":[34.2,33.5,33.6,33.9,34.1,34.7,34.8],"temperature_2m_min":[19.6,20.8,19.9,18.0,17.1,19.2,21.0],"apparent_temperature_max":[36.4,36.3,36.4,36.0,37.0,38.0,37.6],"apparent_temperature_min":[21.8,22.5,21.4,18.8,17.9,20.5,22.6],"precipitation_sum":[0.00,0.00,0.00,0.00,0.00,0.00,0.00]}}""",
            """{"latitude":70.00429,"longitude":19.007751,"generationtime_ms":4.921913146972656,"utc_offset_seconds":3600,"timezone":"Europe/Berlin","timezone_abbreviation":"GMT+1","elevation":212.0,"current_units":{"time":"iso8601","interval":"seconds","temperature_2m":"°C","relative_humidity_2m":"%","apparent_temperature":"°C","is_day":"","precipitation":"mm","rain":"mm","showers":"mm","snowfall":"cm","weather_code":"wmo code","cloud_cover":"%","pressure_msl":"hPa","surface_pressure":"hPa","wind_speed_10m":"km/h","wind_direction_10m":"°","wind_gusts_10m":"km/h"},"current":{"time":"2025-02-05T21:15","interval":900,"temperature_2m":0.9,"relative_humidity_2m":90,"apparent_temperature":-6.7,"is_day":0,"precipitation":0.30,"rain":0.30,"showers":0.00,"snowfall":0.00,"weather_code":55,"cloud_cover":75,"pressure_msl":1008.0,"surface_pressure":981.8,"wind_speed_10m":37.1,"wind_direction_10m":227,"wind_gusts_10m":62.6},"hourly_units":{"time":"iso8601","temperature_2m":"°C","apparent_temperature":"°C","precipitation_probability":"%","weather_code":"wmo code","visibility":"m","wind_speed_10m":"km/h","is_day":""},"hourly":{"time":["2025-02-05T21:00","2025-02-05T22:00","2025-02-05T23:00","2025-02-06T00:00","2025-02-06T01:00","2025-02-06T02:00"],"temperature_2m":[1.0,0.5,0.5,0.6,0.9,0.6],"apparent_temperature":[-6.7,-6.5,-7.2,-7.6,-5.8,-6.5],"precipitation_probability":[78,90,94,94,94,96],"weather_code":[55,2,51,61,53,53],"visibility":[14540.00,23540.00,23240.00,12840.00,21200.00,12060.00],"wind_speed_10m":[37.8,31.7,37.1,41.8,30.6,32.8],"is_day":[0,0,0,0,0,0]},"daily_units":{"time":"iso8601","weather_code":"wmo code","temperature_2m_max":"°C","temperature_2m_min":"°C","apparent_temperature_max":"°C","apparent_temperature_min":"°C","precipitation_sum":"mm"},"daily":{"time":["2025-02-05","2025-02-06","2025-02-07","2025-02-08","2025-02-09","2025-02-10","2025-02-11"],"weather_code":[75,63,61,3,73,73,73],"temperature_2m_max":[1.0,5.0,5.0,3.9,0.1,3.2,3.7],"temperature_2m_min":[-3.5,0.6,3.6,-1.0,-2.7,-1.9,1.7],"apparent_temperature_max":[-5.1,-5.7,-4.8,-1.1,-5.9,-4.2,-3.1],"apparent_temperature_min":[-12.6,-8.1,-6.8,-5.7,-8.6,-7.8,-5.7],"precipitation_sum":[10.40,26.70,13.00,0.00,3.20,4.90,10.30]}}"""
        };
    }
}
