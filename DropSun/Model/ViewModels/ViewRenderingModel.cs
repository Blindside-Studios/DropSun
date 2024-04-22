using ABI.Microsoft.UI.Xaml;
using DropSun.Model.Weather;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DropSun.Model.ViewModels
{
    class ViewRenderingModel : INotifyPropertyChanged
    {
        private static ViewRenderingModel _instance;
        public static ViewRenderingModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ViewRenderingModel();
                }
                return _instance;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Condition WeatherCondition { get; set; }

        private Windows.Foundation.Point _cursorPosition;
        private const float MaxSpeed = 0.25f; // Maximum speed per update
        private float _timeSinceLastUpdate = 0f;
        private const float UpdateTime = 0.016f; // Assuming 60 updates per second

        private bool _sunNeedsUpdate = false;
        private bool _umbrellaNeedsUpdate = false;
        private Windows.Foundation.Point _targetCursorPosition;

        public Windows.Foundation.Point CursorPosition
        {
            get => _cursorPosition;
            set
            {
                if (_cursorPosition != value)
                {
                    _cursorPosition = value;
                    _targetCursorPosition = value;
                    OnPropertyChanged(nameof(CursorPosition));

                    switch (WeatherCondition)
                    {
                        case Condition.Sunny:
                            _sunNeedsUpdate = true;
                            StartUpdatingSunPosition();
                            break;
                        case Condition.Rainy:
                            _umbrellaNeedsUpdate = true;
                            StartUpdatingUmbrellaPosition();
                            break;
                        default:
                            Debug.WriteLine("No condition set");
                            break;
                    }
                }
            }
        }

        private async void StartUpdatingSunPosition()
        {
            while (_sunNeedsUpdate)
            {
                Vector3 newSunTranslation = Vector3.Lerp(SunTranslation, new Vector3((float)_targetCursorPosition.X - 50, (float)_targetCursorPosition.Y - 50, 0), MaxSpeed * UpdateTime);

                // Check if the SunTranslation has reached the target position
                if (Vector3.Distance(newSunTranslation, SunTranslation) < 0.01f) // Use a small threshold to determine if the position is close enough
                {
                    SunTranslation = newSunTranslation;
                    _sunNeedsUpdate = false; // Clear the flag as the target has been reached
                }
                else
                {
                    SunTranslation = newSunTranslation;
                    await Task.Delay(TimeSpan.FromMilliseconds(16)); // Wait for a short duration before the next update
                }
            }
        }

        private async void StartUpdatingUmbrellaPosition()
        {
            while (_umbrellaNeedsUpdate)
            {
                Vector3 newUmbrellaTranslation = Vector3.Lerp(UmbrellaTranslation, new Vector3((float)_targetCursorPosition.X - 120, (float)_targetCursorPosition.Y - 266, 0), MaxSpeed * UpdateTime);

                float distance = UmbrellaTranslation.X - newUmbrellaTranslation.X;
                float rotation = distance * 45;
                rotation = Math.Clamp(rotation, -45, 45);

                UmbrellaRotation = rotation;

                if (Vector3.Distance(newUmbrellaTranslation, UmbrellaTranslation) < 0.01f)
                {
                    UmbrellaTranslation = newUmbrellaTranslation;
                    _umbrellaNeedsUpdate = false;
                }
                else
                {
                    UmbrellaTranslation = newUmbrellaTranslation;
                    await Task.Delay(TimeSpan.FromMilliseconds(16));
                }
            }
        }




        private Vector3 _sunTranslation;
        public Vector3 SunTranslation
        {
            get => _sunTranslation;
            set
            {
                if (_sunTranslation != value)
                {
                    _sunTranslation = value;
                    OnPropertyChanged(nameof(SunTranslation));
                    calculateTimeOfDay(_sunTranslation.Y);
                }
            }
        }

        private Vector3 _umbrellaTranslation;
        public Vector3 UmbrellaTranslation
        {
            get => _umbrellaTranslation;
            set
            {
                if (_umbrellaTranslation != value)
                {
                    _umbrellaTranslation = value;
                    OnPropertyChanged(nameof(UmbrellaTranslation));
                    UmbrellaCenterPoint = new Point((int)UmbrellaTranslation.X + 145, (int)UmbrellaTranslation.Y + 140);
                    // set the trigger 100 up to account for droplets only being registered after a certain time
                    UmbrellaCenterPointVector = new Vector3(UmbrellaCenterPoint.X, UmbrellaCenterPoint.Y - 100, 0);
                }
            }
        }

        private float _umbrellaRotation;
        public float UmbrellaRotation
        {
            get => _umbrellaRotation;
            set
            {
                if (_umbrellaRotation != value)
                {
                    _umbrellaRotation = value;
                    OnPropertyChanged(nameof(UmbrellaRotation));
                }
            }
        }

        private Vector3 _umbrellaCenterPointVector;
        public Vector3 UmbrellaCenterPointVector
        {
            get => _umbrellaCenterPointVector;
            set
            {
                if (_umbrellaCenterPointVector != value)
                {
                    _umbrellaCenterPointVector = value;
                    OnPropertyChanged(nameof(UmbrellaCenterPointVector));
                }
            }
        }

        public Point UmbrellaCenterPoint { get; set; }
        public int UmbrellaRadius
        {
            get => 100;
        }


        // constants defining the day to night curve
        const int eveningStartThreshold = 500;
        // at which point in the evening does the night start?
        const double nightRelativeStartThreshold = 0.75;

        // This stores the height of the grid that tracks mouse movements to calculate the distance to the bottom
        public int ReceiverGridWidth { get; set; }
        public int ReceiverGridHeight { get; set; }

        private void calculateTimeOfDay(double sunYPosition)
        {
            sunYPosition = sunYPosition + 75;

            double nightModfier = 0;
            if (ReceiverGridHeight - sunYPosition < eveningStartThreshold)
            {
                nightModfier = 1 - ((ReceiverGridHeight - sunYPosition) / eveningStartThreshold);
            }

            if (nightModfier > nightRelativeStartThreshold)
            {
                NightLevel = (nightModfier - nightRelativeStartThreshold) / (1 - nightRelativeStartThreshold);
                EveningLevel = 1;
            }
            else if (nightModfier > 0)
            {
                NightLevel = 0;
                EveningLevel = nightModfier / nightRelativeStartThreshold;
            }
            else if (nightModfier == 0)
            {
                NightLevel = 0;
                EveningLevel = 0;
            }
        }

        private double _eveningLevel;
        public double EveningLevel
        {
            get => (double)_eveningLevel;
            set
            {
                if (_eveningLevel != value)
                {
                    _eveningLevel = value;
                    OnPropertyChanged(nameof(EveningLevel));
                }
            }
        }

        private double _nightLevel;
        public double NightLevel
        {
            get => (double)_nightLevel;
            set
            {
                if (_nightLevel != value)
                {
                    _nightLevel = value;
                    OnPropertyChanged(nameof(NightLevel));
                    if (NightLevel == 0) AreStarsRequired = Visibility.Collapsed;
                    else if (NightLevel > 0) AreStarsRequired = Visibility.Visible;
                }
            }
        }

        private Visibility _areStarsRequired;
        public Visibility AreStarsRequired
        {
            get => _areStarsRequired;
            set
            {
                if (_areStarsRequired != value)
                {
                    _areStarsRequired = value;
                    OnPropertyChanged(nameof(AreStarsRequired));
                }
            }
        }


        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
