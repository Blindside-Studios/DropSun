using DropSun.Model.Geolocation;
using DropSun.Model.Weather;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
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
            Model.ViewModels.WeatherState.Instance.Forecast = Weather;
            Model.ViewModels.WeatherState.Instance.Location = Location;
            Pikouna_Engine.WeatherViewModel.Instance.CloudCoverageExternal = Weather.Current.CloudCover;
            Pikouna_Engine.WeatherViewModel.Instance.Showers = Weather.Current.Showers;
            Pikouna_Engine.WeatherViewModel.Instance.WindSpeed = Weather.Current.WindSpeed10M;
            Pikouna_Engine.WeatherViewModel.Instance.Snow = Weather.Current.Snowfall;
            Pikouna_Engine.WeatherViewModel.Instance.WeatherType = (Pikouna_Engine.WeatherType)Weather.GetWeatherDescription(); // casting between these works because they have the same names
        }

        private void SidebarWeatherItem_Loaded(object sender, RoutedEventArgs e)
        {
            //SkyFrame.NavigateToType(typeof(Views.Conditions.Sunny.BlueSky), null, null);
        }

        public Model.Geolocation.InternalGeolocation Location
        {
            get { return (Model.Geolocation.InternalGeolocation)GetValue(LocationProperty); }
            set { 
                SetValue(LocationProperty, value); 
                LocationTextBox.Text = value.name; 
                SubLocationTextBox.Text = GeoLookup.GetCountryName(Location.country_code);
                ToolTip toolTip = new ToolTip();
                toolTip.Content = $"{Location.name}, {GeoLookup.GetStateName(Location.country_code, Location.state_code)}, {GeoLookup.GetCountryName(Location.country_code)}";
                ToolTipService.SetToolTip(LocationStackPanel, toolTip);
            }
        }
        public static readonly DependencyProperty LocationProperty =
            DependencyProperty.Register("Location", typeof(string), typeof(SidebarWeatherItem), new PropertyMetadata(default(Model.Geolocation.InternalGeolocation)));

        public double Temperature
        {
            get { return (double)GetValue(TemperatureProperty); }
            set { SetValue(TemperatureProperty, value);  }
        }
        public static readonly DependencyProperty TemperatureProperty =
            DependencyProperty.Register("Temperature", typeof(double), typeof(SidebarWeatherItem), new PropertyMetadata(default(double)));

        public int Precipitation
        {
            get { return (int)GetValue(PrecipitationProperty); }
            set { SetValue(PrecipitationProperty, value); }
        }
        public static readonly DependencyProperty PrecipitationProperty =
            DependencyProperty.Register("Precipitation", typeof(int), typeof(SidebarWeatherItem), new PropertyMetadata(default(int)));

        public OpenMeteoWeatherOverview Weather
        {
            get { return (Model.Weather.OpenMeteoWeatherOverview)GetValue(WeatherProperty); }
            set {
                SetValue(WeatherProperty, value);

                TemperatureTextBox.Text = ((double)value.Current.Temperature2M).ToString() + value.CurrentUnits.Temperature2M[0];
                
                ResourceLoader _resourceLoader = ResourceLoader.GetForViewIndependentUse();
                ResourceLoader _conditionsResourceLoader = ResourceLoader.GetForViewIndependentUse("Conditions");

                double minTemperature = (double)Math.Round(value.Daily.ApparentTemperatureMin[0]);
                double maxTemperature = (double)Math.Round(value.Daily.ApparentTemperatureMax[0]);
                char temperatureUnit = value.CurrentUnits.Temperature2M[0];
                HighLowTextBox.Text = string.Format(_resourceLoader.GetString("Weather/HighLowShort"), maxTemperature, minTemperature, temperatureUnit);
                ConditionsTextBox.Text = _conditionsResourceLoader.GetString(value.GetWeatherDescription().ToString());

                System.Drawing.Color color = new();

                string imagePath = $"ms-appx:///Assets/WeatherPreviews/{value.GetWeatherDescription().ToString()}.png";
                WeatherPreview.Source = new BitmapImage(new Uri(imagePath));

                switch (value.Current.WeatherCode)
                {
                    // taken these code from here https://open-meteo.com/en/docs#:~:text=WMO%20Weather%20interpretation%20codes%20(WW)
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                        // clear
                        color = System.Drawing.Color.DodgerBlue;
                        break;
                    case 45:
                    case 48:
                        // fog
                        color = System.Drawing.Color.DeepSkyBlue;
                        break;
                    case 51:
                    case 53:
                    case 55:
                        // drizzle
                        color = System.Drawing.Color.SlateGray;
                        break;
                    case 56:
                    case 57:
                        // freezing drizzle
                        color = System.Drawing.Color.DimGray;
                        break;
                    case 61:
                    case 63:
                    case 65:
                        // rain
                        color = System.Drawing.Color.SteelBlue;
                        break;
                    case 66:
                    case 67:
                        // freezing rain
                        color = System.Drawing.Color.DarkSlateBlue;
                        break;
                    case 71:
                    case 73:
                    case 75:
                        // snow fall
                        color = System.Drawing.Color.LightGray;
                        break;
                    case 77:
                        // snow grains
                        color = System.Drawing.Color.LightSlateGray;
                        break;
                    default:
                        color = System.Drawing.Color.DarkGray;
                        break;
                }
                BackgroundColor.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(color.A, color.R, color.G, color.B));
                checkForTrimming();
            }
        }
        public static readonly DependencyProperty WeatherProperty =
            DependencyProperty.Register("Weather", typeof(OpenMeteoWeatherOverview), typeof(SidebarWeatherItem), new PropertyMetadata(default(OpenMeteoWeatherOverview)));
    
        private async void checkForTrimming()
        {
            // separate method because you can't make the setter async - hacky workaround number 3244
            await Task.Delay(100);
            if (ConditionsTextBox.IsTextTrimmed)
            {
                ResourceLoader _resourceLoader = ResourceLoader.GetForViewIndependentUse();

                double minTemperature = (double)Math.Round(Weather.Daily.ApparentTemperatureMin[0]);
                double maxTemperature = (double)Math.Round(Weather.Daily.ApparentTemperatureMax[0]);
                char temperatureUnit = Weather.CurrentUnits.Temperature2M[0];
                string highString = string.Format(_resourceLoader.GetString("Weather/HighLowMinimal"), maxTemperature, temperatureUnit);
                string highLowString = string.Format(_resourceLoader.GetString("Weather/HighLowShort"), maxTemperature, minTemperature, temperatureUnit);

                HighLowTextBox.Text = highString;
                ToolTip temperatureTooltip = new ToolTip();
                temperatureTooltip.Content = highLowString;
                ToolTipService.SetToolTip(HighLowTextBox, temperatureTooltip);

                await Task.Delay(100);
                if (ConditionsTextBox.IsTextTrimmed)
                {
                    HighLowTextBox.Text = "";
                    ToolTipService.SetToolTip(TemperatureTextBox, temperatureTooltip);

                    await Task.Delay(100);
                    if (ConditionsTextBox.IsTextTrimmed)
                    {
                        ToolTip toolTip = new ToolTip();
                        toolTip.Content = ConditionsTextBox.Text;
                        ToolTipService.SetToolTip(ConditionsTextBox, toolTip);
                    }
                }
                else { ToolTipService.SetToolTip(ConditionsTextBox, null); ToolTipService.SetToolTip(TemperatureTextBox, null); }
            }
            else { ToolTipService.SetToolTip(ConditionsTextBox, null); ToolTipService.SetToolTip(TemperatureTextBox, null); ToolTipService.SetToolTip(HighLowTextBox, null); }
        }
    }
}
