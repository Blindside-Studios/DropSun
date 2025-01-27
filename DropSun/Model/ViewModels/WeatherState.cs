using DropSun.Model.Weather;
using OpenMeteo;
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


        private Condition _condition;
        public Condition Condition
        {
            get => _condition;
            set
            {
                if (_condition != value)
                {
                    _condition = value;
                    OnPropertyChanged(nameof(Condition));
                    switch (_condition)
                    {
                        case Condition.Sunny:
                            ConditionDescription = "Sunny";
                            break;
                        case Condition.Rainy:
                            ConditionDescription = "Rainy";
                            break;
                        default:
                            ConditionDescription = "Unknown Condition";
                            break;
                    }
                }
            }
        }


        private string _conditionDescription;
        public string ConditionDescription
        {
            get => _conditionDescription;
            set
            {
                if (_conditionDescription != value)
                {
                    _conditionDescription = value;
                    OnPropertyChanged(nameof(ConditionDescription));
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
