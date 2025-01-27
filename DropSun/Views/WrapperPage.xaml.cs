using DropSun.Model.Geolocation;
using DropSun.Model.Weather;
using DropSun.Views.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DropSun.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
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
            //weatherItem.Weather = new Weather()
            //{
            //    Forecast = null
            //};
            LocationsListView.Items.Add(weatherItem);

            var weatherForecast = await OpenMeteoAPI.GetWeatherAsync(SelectedLocation.latitude, SelectedLocation.longitude);
            //Debug.WriteLine(weatherForecast.ToString());
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
            if (isSideBarExpanded) collapseSidebar();
            else expandSidebar();
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

        private void ShowSunnyButton_Click(object sender, RoutedEventArgs e)
        {
            Model.ViewModels.WeatherState.Instance.Forecast = null;
            FrameNavigationOptions navOptions = new FrameNavigationOptions();
            navOptions.TransitionInfoOverride = new DrillInNavigationTransitionInfo();
            Type pageType = typeof(WeatherView);
            ContentFrame.NavigateToType(pageType, null, navOptions);
        }

        private void ShowRainButton_Click(object sender, RoutedEventArgs e)
        {
            Model.ViewModels.WeatherState.Instance.Forecast = null;
            FrameNavigationOptions navOptions = new FrameNavigationOptions();
            navOptions.TransitionInfoOverride = new DrillInNavigationTransitionInfo();
            Type pageType = typeof(WeatherView);
            ContentFrame.NavigateToType(pageType, null, navOptions);
        }

        private async void ConfirmCityButton_Click(object sender, RoutedEventArgs e)
        {
            //await Model.Weather.ObtainWeather.FromOpenMeteo(ExampleCityTextBox.Text);
        }

        private void LocationsListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (ExampleCityTextBox.Visibility == Visibility.Collapsed)
            {
                ExampleCityTextBox.Visibility = Visibility.Visible;
                ConfirmCityButton.Visibility = Visibility.Visible;
                ShowSunnyButton.Visibility = Visibility.Visible;
                ShowRainButton.Visibility = Visibility.Visible;
            }
            else
            {
                ExampleCityTextBox.Visibility = Visibility.Collapsed;
                ConfirmCityButton.Visibility = Visibility.Collapsed;
                ShowSunnyButton.Visibility = Visibility.Collapsed;
                ShowRainButton.Visibility = Visibility.Collapsed;
            }
        }
    }
}
