using ABI.Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public Windows.Foundation.Point CursorPosition
        {
            get => _cursorPosition;
            set
            {
                if (_cursorPosition != value)
                {
                    _cursorPosition = value;
                    OnPropertyChanged(nameof(CursorPosition));
                    SunTranslation = new Vector3((float)_cursorPosition.X - 50, (float)_cursorPosition.Y - 50, 0);
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
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
