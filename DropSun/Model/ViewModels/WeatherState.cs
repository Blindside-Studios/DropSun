using DropSun.Model.Weather;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropSun.Model.ViewModels
{
    internal class WeatherState: INotifyPropertyChanged
    {
        private static WeatherState _instance;
        public static WeatherState Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new WeatherState();
                }
                return _instance;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private WeatherResponse _forecast;
        public WeatherResponse Forecast
        {
            get => _forecast;
            set
            {
                if (_forecast != value)
                {
                    _forecast = value;
                    OnPropertyChanged(nameof(Forecast));
                }
            }
        }

        private double _temperatureDouble;
        public double TemperatureDouble
        {
            get => _temperatureDouble;
            set
            {
                if (_temperatureDouble != value)
                {
                    _temperatureDouble = value;
                    OnPropertyChanged(nameof(TemperatureDouble));
                    TemperatureString = TemperatureDouble.ToString() + " °C";
                }
            }
        }

        private string _temperatureString;
        public string TemperatureString
        {
            get => _temperatureString;
            set
            {
                if (_temperatureString != value)
                {
                    _temperatureString = value;
                    OnPropertyChanged(nameof(TemperatureString));
                }
            }
        }
    }
}
