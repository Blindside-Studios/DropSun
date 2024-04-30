using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DropSun.Views.Controls
{
    public sealed partial class SidebarWeatherItem : UserControl
    {
        public SidebarWeatherItem()
        {
            this.InitializeComponent();
            this.Loaded += SidebarWeatherItem_Loaded;
            this.PointerPressed += SidebarWeatherItem_PointerPressed;
        }

        private void SidebarWeatherItem_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Model.ViewModels.WeatherState.Instance.Forecast = Weather.Forecast;
            Model.ViewModels.WeatherState.Instance.Condition = Weather.Conditions;
        }

        private void SidebarWeatherItem_Loaded(object sender, RoutedEventArgs e)
        {
            //SkyFrame.NavigateToType(typeof(Views.Conditions.Sunny.BlueSky), null, null);
        }

        public string Location
        {
            get { return (string)GetValue(LocationProperty); }
            set { SetValue(LocationProperty, value); LocationTextBox.Text = value; }
        }
        public static readonly DependencyProperty LocationProperty =
            DependencyProperty.Register("Location", typeof(string), typeof(SidebarWeatherItem), new PropertyMetadata(default(string)));

        public double Temperature
        {
            get { return (double)GetValue(TemperatureProperty); }
            set { SetValue(TemperatureProperty, value); TemperatureTextBox.Text = value.ToString("0.0") + "°C"; }
        }
        public static readonly DependencyProperty TemperatureProperty =
            DependencyProperty.Register("Temperature", typeof(double), typeof(SidebarWeatherItem), new PropertyMetadata(default(double)));

        public int Precipitation
        {
            get { return (int)GetValue(PrecipitationProperty); }
            set { SetValue(PrecipitationProperty, value); PrecipitationTextBox.Text = Precipitation.ToString("0") + "%"; }
        }
        public static readonly DependencyProperty PrecipitationProperty =
            DependencyProperty.Register("Precipitation", typeof(int), typeof(SidebarWeatherItem), new PropertyMetadata(default(int)));

        public Model.Weather.Weather Weather
        {
            get { return (Model.Weather.Weather)GetValue(WeatherProperty); }
            set { 
                SetValue(WeatherProperty, value);

                System.Drawing.Color color = new();
                
                switch (value.Conditions)
                {
                    case Model.Weather.Condition.NotYetAvailable:
                        break;
                    case Model.Weather.Condition.Sunny:
                        color = System.Drawing.Color.DodgerBlue;
                        break;
                    case Model.Weather.Condition.Rainy:
                        color = System.Drawing.Color.DarkGray;
                        break;
                    default:
                        break;
                }

                BackgroundColor.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(color.A, color.R, color.G, color.B));
            }
        }
        public static readonly DependencyProperty WeatherProperty =
            DependencyProperty.Register("Weather", typeof(Model.Weather.Weather), typeof(SidebarWeatherItem), new PropertyMetadata(default(Model.Weather.Weather)));
    }
}
