using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Pikouna_Engine;
using DropSun.Model.ViewModels;
using Windows.Data.Xml.Dom;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DropSun.Views.Application.SettingsPages
{
    public sealed partial class GeneralSettingsPage : Page
    {
        public GeneralSettingsPage()
        {
            this.InitializeComponent();
            this.Loaded += GeneralSettingsPage_Loaded;
            AppSettings.Instance.PropertyChanged += Instance_PropertyChanged;
        }

        private void loadSettings()
        {
            switch (AppSettings.Instance.Units)
            {
                case MeasurementUnits.GetFromRegion:
                    UnitsRadioButtonGroup.SelectedIndex = 0;
                    break;
                case MeasurementUnits.Metric:
                    UnitsRadioButtonGroup.SelectedIndex = 1;
                    break;
                case MeasurementUnits.Imperial:
                    UnitsRadioButtonGroup.SelectedIndex = 2;
                    break;
            }
        }

        private void GeneralSettingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            loadSettings();
        }

        private void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AppSettings.Instance.Units)) loadSettings();
        }

        private void GetFromSystemRegionRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            AppSettings.Instance.Units = MeasurementUnits.GetFromRegion;
        }

        private void MetricRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            AppSettings.Instance.Units = MeasurementUnits.Metric;
        }

        private void ImperialRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            AppSettings.Instance.Units = MeasurementUnits.Imperial;
        }
    }
}
