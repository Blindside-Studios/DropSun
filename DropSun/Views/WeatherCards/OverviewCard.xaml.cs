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
using Windows.UI.Composition;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml.Hosting;
using DropSun.Model.ViewModels;
using Windows.ApplicationModel.Resources;
using DropSun.Model.Geolocation;
using DropSun.Model.Weather;
using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DropSun.Views.WeatherCards
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OverviewCard : Page
    {
        public OverviewCard()
        {
            this.InitializeComponent();
            this.Loaded += OverviewCard_Loaded;
        }

        private void OverviewCard_Loaded(object sender, RoutedEventArgs e)
        {
            WeatherState.Instance.PropertyChanged += Instance_PropertyChanged;
            updateWeatherCards();
        }

        private void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            updateWeatherCards();
        }

        private void updateWeatherCards()
        {
            if (WeatherState.Instance.Forecast != null && WeatherState.Instance.Location != null)
            {
                ResourceLoader _loader = ResourceLoader.GetForViewIndependentUse();
                ResourceLoader _conditionsLoader = ResourceLoader.GetForViewIndependentUse("Conditions");

                CountryTextBlock.Text = GeoLookup.GetCountryName(WeatherState.Instance.Location.country_code);
                TemperatureTextBlock.Text = $"{WeatherState.Instance.Forecast.Current.Temperature2M}{WeatherState.Instance.Forecast.CurrentUnits.Temperature2M[0]}";
                string apparentTemperature = string.Format(_loader.GetString("ApparentTemperatureString"),
                    WeatherState.Instance.Forecast.Current.ApparentTemperature,
                    WeatherState.Instance.Forecast.CurrentUnits.ApparentTemperature[0].ToString());
                ApparentTemperatureTextBlock.Text = apparentTemperature;
                ConditionsTextBox.Text = _conditionsLoader.GetString(WeatherState.Instance.Forecast.GetWeatherDescription().ToString());
            }
        }
    }
}
