using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropSun.Model.Weather
{
    public class Weather : INotifyPropertyChanged
    {
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
