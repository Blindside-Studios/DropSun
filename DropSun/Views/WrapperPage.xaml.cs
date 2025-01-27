using DropSun.Model.Geolocation;
using DropSun.Model.Weather;
using DropSun.Views.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;

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

        public async void addLocation(InternalGeolocation SelectedLocation)
        {
            SidebarWeatherItem weatherItem = new()
            {
                Location = SelectedLocation.name,
                Temperature = 0,
                Precipitation = 0,
            };
            LocationsListView.Items.Add(weatherItem);

            var weatherForecast = await OpenMeteoAPI.GetWeatherAsync(SelectedLocation.latitude, SelectedLocation.longitude);
            weatherItem.Weather = weatherForecast;
            weatherItem.Temperature = (double)weatherForecast.Current.Temperature2M;
            weatherItem.Precipitation = (int)weatherForecast.Current.Precipitation;
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
            LocationsListView.Items.Add(weatherItem);
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
            doubleAnimation.To = 270;
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
            doubleAnimation.To = 20;
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
    }
}
