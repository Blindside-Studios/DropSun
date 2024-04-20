using ABI.Microsoft.UI.Xaml;
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
    class ViewRenderingModel: INotifyPropertyChanged
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


        private Windows.Foundation.Point _cursorPosition;
        private const float MaxSpeed = 0.25f; // Maximum speed per update
        private float _timeSinceLastUpdate = 0f;
        private const float UpdateTime = 0.016f; // Assuming 60 updates per second

        private bool _sunNeedsUpdate = false;
        private Windows.Foundation.Point _targetCursorPosition;

        public Windows.Foundation.Point CursorPosition
        {
            get => _cursorPosition;
            set
            {
                if (_cursorPosition != value)
                {
                    _cursorPosition = value;
                    _targetCursorPosition = value; // Update the target position
                    OnPropertyChanged(nameof(CursorPosition));
                    _sunNeedsUpdate = true; // Set the flag to true as the cursor has moved
                    StartUpdatingSunPosition(); // Start the update process
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


        // constants defining the day to night curve
        const int eveningStartThreshold = 500;
        // at which point in the evening does the night start?
        const double nightRelativeStartThreshold = 0.75;

        // This stores the height of the grid that tracks mouse movements to calculate the distance to the bottom
        public int ReceiverGridHeight { get; set; }

        private void calculateTimeOfDay(double sunYPosition)
        {
            sunYPosition = sunYPosition + 50;

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
                }
            }
        }


        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
