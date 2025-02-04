using DropSun.Model.Geolocation;
using DropSun.Model.Weather;
using DropSun.Views.Controls;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;

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
        }

        private void WrapperPage_Loaded(object sender, RoutedEventArgs e)
        {
            ContentFrame.CornerRadius = new Microsoft.UI.Xaml.CornerRadius(8, 8, 3, 8);
            FrameNavigationOptions navOptions = new FrameNavigationOptions();
            navOptions.TransitionInfoOverride = new DrillInNavigationTransitionInfo();
            Type pageType = typeof(WeatherView);
            ContentFrame.NavigateToType(pageType, null, navOptions);
            
        }

        public async void addLocation(InternalGeolocation SelectedLocation, bool useAnimation)
        {
            SidebarWeatherItem weatherItem = new()
            {
                Location = SelectedLocation.name,
                Temperature = 0,
                Precipitation = 0,
            };

            Frame frame = new Frame();
            frame.Content = weatherItem;

            var weatherForecast = await OpenMeteoAPI.GetWeatherAsync(SelectedLocation.latitude, SelectedLocation.longitude);
            weatherItem.Weather = weatherForecast;
            weatherItem.Temperature = (double)weatherForecast.Current.Temperature2M;
            weatherItem.Precipitation = (int)weatherForecast.Current.Precipitation;

            if (!useAnimation) LocationsStackPanel.Children.Add(frame);
            else animateItem(frame);
        }

        private void animateItem(Frame frame)
        {
            frame.Opacity = 1; // change this to 0 later for an animation
            LocationsStackPanel.Children.Add(frame);

            animateAddedItem(frame);
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
            translateYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0.8), Value = 0, EasingFunction = new BackEase { EasingMode = EasingMode.EaseInOut, Amplitude = 0.5 } });

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

            await Task.Delay(350);

            int i = 1;
            foreach(Frame otherFrame in listOfFrames)
            {
                rippleOtherItems(otherFrame, i);
                await Task.Delay(200);
                i++;
            }
        }

        private void rippleOtherItems(Frame frame, int index)
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
            DoubleAnimationUsingKeyFrames translateYAnimation = new DoubleAnimationUsingKeyFrames { EnableDependentAnimation = true };

            scaleXAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0), Value = 1 });
            scaleXAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0.3), Value = 1 - 0.1/index, EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut } });
            scaleXAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(1), Value = 1, EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 0.3 } });

            scaleYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0), Value = 1 });
            scaleYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0.3), Value = 1 - 0.1/index, EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut } });
            scaleYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(1), Value = 1, EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 0.3 } });

            translateYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0), Value = 0 });
            translateYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0.25), Value = -5 / index, EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut } });
            translateYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(1), Value = 0, EasingFunction = new BackEase { EasingMode = EasingMode.EaseInOut, Amplitude = 0.2 } });

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

        public void addDebugLocation(string location)
        {
            SidebarWeatherItem weatherItem = new()
            {
                Location = location,
                Temperature = 20.5,
                Precipitation = 21,
            };
            weatherItem.Weather = null;
            LocationsStackPanel.Children.Add(weatherItem);
        }

        public void toggleSidebarState()
        {
            if (isSideBarExpanded)
            {
                collapseSidebar();
                ContentFrame.CornerRadius = new Microsoft.UI.Xaml.CornerRadius(8, 8, 3, 3);
            }
            else
            {
                expandSidebar();
                ContentFrame.CornerRadius = new Microsoft.UI.Xaml.CornerRadius(8, 8, 3, 8);
            }
        }

        public void expandSidebar()
        {
            isSideBarExpanded = true;
            if (currentAnimation != null)
            {
                currentAnimation.Stop();
            }
            
            Duration duration = new Duration(TimeSpan.FromSeconds(0.4));
            BackEase backEase = new BackEase();
            backEase.EasingMode = EasingMode.EaseOut;
            backEase.Amplitude = 0.4;

            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.Duration = duration;
            doubleAnimation.From = SidebarContainer.ActualWidth;
            doubleAnimation.To = 275;
            doubleAnimation.EnableDependentAnimation = true;
            doubleAnimation.EasingFunction = backEase;

            Storyboard sb = new Storyboard();
            sb.Duration = duration;
            sb.Children.Add(doubleAnimation);

            Storyboard.SetTarget(doubleAnimation, SidebarContainer);
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("Width").Path);

            currentAnimation = sb;
            sb.Begin();
        }

        public void collapseSidebar()
        {
            isSideBarExpanded = false;
            if (currentAnimation != null)
            {
                currentAnimation.Stop();
            }

            Duration duration = new Duration(TimeSpan.FromSeconds(0.3));
            var ease = new CircleEase();
            ease.EasingMode = EasingMode.EaseInOut;

            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.Duration = duration;
            doubleAnimation.From = SidebarContainer.ActualWidth;
            doubleAnimation.To = 25;
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

        private void SidebarContainer_RightTapped(object sender, Microsoft.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            Random rnd = new Random();
            SidebarWeatherItem weatherItem = new()
            {
                Location = $"Location {LocationsStackPanel.Children.Count().ToString()}",                
            };

            Frame frame = new Frame();
            frame.Content = weatherItem;

            var weatherForecast = new OpenMeteoWeatherOverview { 
                Current = new CurrentWeather { WeatherCode = 0, Temperature2M = 15, Precipitation = 0 },
                CurrentUnits = new CurrentUnits { Temperature2M = "°C", Precipitation = "mm" }
            };
            weatherItem.Weather = weatherForecast;
            weatherItem.Temperature = 5;
            weatherItem.Precipitation = 0;
            
            animateItem(frame);
        }

        private void SidebarContainer_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            LocationsStackPanel.Children.Clear();
        }
    }
}
