using DropSun.Model.ViewModels;
using DropSun.Views.WeatherCards;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
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
    public sealed partial class WeatherView : Page
    {
        public WeatherView()
        {
            this.InitializeComponent();
            this.Loaded += WeatherView_Loaded;
            WeatherState.Instance.PropertyChanged += Weather_PropertyChanged;
        }

        private void Weather_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(WeatherState.Instance.Condition):
                    updateBackground();
                    break;
                case nameof(WeatherState.Instance.Forecast):
                    // update weather display here
                    break;
                default:
                    break;
            }
        }

        private void WeatherView_Loaded(object sender, RoutedEventArgs e)
        {
            updateBackground();
        }

        private void updateBackground()
        {
            ViewRenderingModel.Instance.ReceiverGridHeight = Convert.ToInt32(ContentGrid.ActualHeight);
            ViewRenderingModel.Instance.ReceiverGridWidth = Convert.ToInt32(ContentGrid.ActualWidth);

            FrameNavigationOptions navOptions = new FrameNavigationOptions();
            if (WeatherState.Instance.Condition == Model.Weather.Condition.Sunny)
            {

                Type pageType = typeof(Conditions.Rendered.Sunny);
                ContentFrame.NavigateToType(pageType, null, navOptions);
            }
            else
            {
                Type pageType = typeof(Conditions.Rendered.Rainy);
                ContentFrame.NavigateToType(pageType, null, navOptions);
            }

            GeneralFrame.Navigate(typeof(General), this);

            WeatherState.Instance.Condition = WeatherState.Instance.Condition;
            WeatherState.Instance.TemperatureDouble = 23.5;
        }

        private void ContentGrid_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            // first update the stored grid height and width in case something changed
            ViewRenderingModel.Instance.ReceiverGridHeight = Convert.ToInt32(ContentGrid.ActualHeight);
            ViewRenderingModel.Instance.ReceiverGridWidth = Convert.ToInt32(ContentGrid.ActualWidth);
            // then update the ViewModel with new information, for which it needs the information in the above line
            ViewRenderingModel.Instance.CursorPosition = e.GetCurrentPoint(ContentGrid).Position;
        }
    }
}
