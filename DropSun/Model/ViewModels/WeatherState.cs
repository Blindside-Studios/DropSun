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

        private OpenMeteoWeatherOverview _forecast;
        public OpenMeteoWeatherOverview Forecast
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

        private Geolocation.InternalGeolocation _location;
        public Geolocation.InternalGeolocation Location
        {
            get => _location;
            set
            {
                if (_location != value)
                {
                    _location = value;
                    OnPropertyChanged(nameof(Location));
                }
            }
        }
    }
}
