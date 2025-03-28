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
using DropSun.Model.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DropSun.Views.Application.SettingsPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class VisualSettingsPage : Page
    {
        public VisualSettingsPage()
        {
            this.InitializeComponent();
            this.Loaded += VisualSettingsPage_Loaded;
            AppSettings.Instance.PropertyChanged += Instance_PropertyChanged;
        }

        private void loadSettings()
        {
            switch (AppSettings.Instance.InteractionStyle)
            {
                case Pikouna_Engine.SunInteractionStyle.None:
                    InteractionsRadioButtonGroup.SelectedIndex = 0;
                    break;
                case Pikouna_Engine.SunInteractionStyle.Gentle:
                    InteractionsRadioButtonGroup.SelectedIndex = 1;
                    break;
                case Pikouna_Engine.SunInteractionStyle.Bouncy:
                    InteractionsRadioButtonGroup.SelectedIndex = 2;
                    break;
            }

            switch (AppSettings.Instance.Framerate)
            {
                case 30:
                    FramerateRadioButtonGroup.SelectedIndex = 0;
                    break;
                case 60:
                    FramerateRadioButtonGroup.SelectedIndex = 1;
                    break;
                case 120:
                    FramerateRadioButtonGroup.SelectedIndex = 2;
                    break;
                case 144:
                    FramerateRadioButtonGroup.SelectedIndex = 3;
                    break;
                case 240:
                    FramerateRadioButtonGroup.SelectedIndex = 4;
                    break;
                case 480:
                    FramerateRadioButtonGroup.SelectedIndex = 5;
                    break;
            }
        }

        private void VisualSettingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            loadSettings();
        }

        private void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AppSettings.Instance.InteractionStyle)) loadSettings();
        }

        private void OffRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            AppSettings.Instance.InteractionStyle = Pikouna_Engine.SunInteractionStyle.None;
        }

        private void SlowRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            AppSettings.Instance.InteractionStyle = Pikouna_Engine.SunInteractionStyle.Gentle;
        }

        private void BouncyRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            AppSettings.Instance.InteractionStyle = Pikouna_Engine.SunInteractionStyle.Bouncy;
        }

        private void FramerateRadioButtonGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (FramerateRadioButtonGroup.SelectedIndex)
            {
                case 0:
                    AppSettings.Instance.Framerate = 30;
                    break;
                case 1:
                    AppSettings.Instance.Framerate = 60;
                    break;
                case 2:
                    AppSettings.Instance.Framerate = 120;
                    break;
                case 3:
                    AppSettings.Instance.Framerate = 144;
                    break;
                case 4:
                    AppSettings.Instance.Framerate = 240;
                    break;
                case 5:
                    AppSettings.Instance.Framerate = 480;
                    break;
            }
        }
    }
}
