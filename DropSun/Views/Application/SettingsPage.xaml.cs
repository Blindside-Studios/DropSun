using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
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

namespace DropSun.Views.Application
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
            NavListView.SelectedIndex = 0; // Select General by default
        }

        private void NavListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NavListView.SelectedItem is ListViewItem selectedItem)
            {
                string selectedTag = selectedItem.Tag as string;

                switch (selectedTag)
                {
                    case "general":
                        ContentFrame.Navigate(typeof(SettingsPages.GeneralSettingsPage), null, new DrillInNavigationTransitionInfo());
                        break;
                    case "visual":
                        ContentFrame.Navigate(typeof(SettingsPages.VisualSettingsPage), null, new DrillInNavigationTransitionInfo());
                        break;
                    case "about":
                        ContentFrame.Navigate(typeof(SettingsPages.InfoSettingsPage), null, new DrillInNavigationTransitionInfo());
                        break;
                }
            }
        }
    }
}
