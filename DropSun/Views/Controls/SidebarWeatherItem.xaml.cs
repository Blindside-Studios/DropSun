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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DropSun.Views.Controls
{
    public sealed partial class SidebarWeatherItem : UserControl
    {
        public SidebarWeatherItem()
        {
            this.InitializeComponent();
        }

        public string Location
        {
            get { return (string)GetValue(LocationProperty); }
            set { SetValue(LocationProperty, value); LocationTextBox.Text = value; }
        }
        public static readonly DependencyProperty LocationProperty =
            DependencyProperty.Register("Location", typeof(string), typeof(SidebarWeatherItem), new PropertyMetadata(default(string)));

        public double Temperature
        {
            get { return (double)GetValue(TemperatureProperty); }
            set { SetValue(TemperatureProperty, value); TemperatureTextBox.Text = value.ToString(); }
        }
        public static readonly DependencyProperty TemperatureProperty =
            DependencyProperty.Register("Temperature", typeof(double), typeof(SidebarWeatherItem), new PropertyMetadata(default(double)));

        public int Precipitation
        {
            get { return (int)GetValue(PrecipitationProperty); }
            set { SetValue(PrecipitationProperty, value); PrecipitationTextBox.Text = Precipitation.ToString(); }
        }
        public static readonly DependencyProperty PrecipitationProperty =
            DependencyProperty.Register("Precipitation", typeof(int), typeof(SidebarWeatherItem), new PropertyMetadata(default(int)));

        public Model.Weather.Condition Condition
        {
            get { return (Model.Weather.Condition)GetValue(ConditionProperty); }
            set { SetValue(ConditionProperty, value); }
        }
        public static readonly DependencyProperty ConditionProperty =
            DependencyProperty.Register("Condition", typeof(Model.Weather.Condition), typeof(SidebarWeatherItem), new PropertyMetadata(default(Model.Weather.Condition)));
    }
}
