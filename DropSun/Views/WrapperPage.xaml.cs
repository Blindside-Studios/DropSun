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
        public WrapperPage()
        {
            this.InitializeComponent();
        }

        private void ShowSunnyButton_Click(object sender, RoutedEventArgs e)
        {
            Model.ViewModels.ViewRenderingModel.Instance.WeatherCondition = Model.Weather.Condition.Sunny;
            FrameNavigationOptions navOptions = new FrameNavigationOptions();
            navOptions.TransitionInfoOverride = new DrillInNavigationTransitionInfo();
            Type pageType = typeof(WeatherView);
            ContentFrame.NavigateToType(pageType, null, navOptions);
        }

        private void ShowRainButton_Click(object sender, RoutedEventArgs e)
        {
            Model.ViewModels.ViewRenderingModel.Instance.WeatherCondition = Model.Weather.Condition.Rainy;
            FrameNavigationOptions navOptions = new FrameNavigationOptions();
            navOptions.TransitionInfoOverride = new DrillInNavigationTransitionInfo();
            Type pageType = typeof(WeatherView);
            ContentFrame.NavigateToType(pageType, null, navOptions);
        }

        private void ConfirmCityButton_Click(object sender, RoutedEventArgs e)
        {
            Model.Weather.ObtainWeather.FromOpenMeteo(ExampleCityTextBox.Text);
        }
    }
}
