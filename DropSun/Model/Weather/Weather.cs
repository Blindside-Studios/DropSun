using OpenMeteo;
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
        private Condition _conditions;
        public Condition Conditions
        {
            get => _conditions;
            set
            {
                if (_conditions != value)
                {
                    _conditions = value;
                    OnPropertyChanged(nameof(Conditions));
                }
            }
        }

        private WeatherForecast _forecast;
        public WeatherForecast Forecast
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

        public static Condition getCondition(WeatherForecast forecast)
        {
            return Condition.NotYetAvailable;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
