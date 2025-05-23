using DropSun.Model.Geolocation;
using DropSun.Model.ViewModels;
using DropSun.Views;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DropSun
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private AppWindow m_AppWindow;
        private AppSettings settingsLoader = AppSettings.Instance; // fire this to build the view model instance and load the settings

        public MainWindow()
        {
            this.InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            Activated += MainWindow_Activated;

            m_AppWindow = this.AppWindow;
            AppTitleBar.Loaded += AppTitleBar_Loaded;
            AppTitleBar.SizeChanged += AppTitleBar_SizeChanged;
            ExtendsContentIntoTitleBar = true;

            FrameNavigationOptions navOptions = new FrameNavigationOptions();
            Type pageType = typeof(WrapperPage);
            ContentFrame.NavigateToType(pageType, null, navOptions);
        }

        private void AppTitleBar_Loaded(object sender, RoutedEventArgs e)
        {
            if (ExtendsContentIntoTitleBar == true)
            {
                // Set the initial interactive regions.
                SetRegionsForCustomTitleBar();
                m_AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
            }
        }

        private void AppTitleBar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ExtendsContentIntoTitleBar == true)
            {
                // Update interactive regions if the size of the window changes.
                SetRegionsForCustomTitleBar();
                m_AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
            }
        }

        private void SetRegionsForCustomTitleBar()
        {
            // Specify the interactive regions of the title bar.

            double scaleAdjustment = AppTitleBar.XamlRoot.RasterizationScale;

            RightPaddingColumn.Width = new GridLength(m_AppWindow.TitleBar.RightInset / scaleAdjustment);
            LeftPaddingColumn.Width = new GridLength(m_AppWindow.TitleBar.LeftInset / scaleAdjustment);

            GeneralTransform transform = TitleBarSearchBox.TransformToVisual(null);
            Rect bounds = transform.TransformBounds(new Rect(0, 0,
                                                             TitleBarSearchBox.ActualWidth,
                                                             TitleBarSearchBox.ActualHeight));
            Windows.Graphics.RectInt32 SearchBoxRect = GetRect(bounds, scaleAdjustment);

            transform = SettingsButton.TransformToVisual(null);
            bounds = transform.TransformBounds(new Rect(0, 0,
                                                        SettingsButton.ActualWidth,
                                                        SettingsButton.ActualHeight));
            Windows.Graphics.RectInt32 SettingsBtnRect = GetRect(bounds, scaleAdjustment);

            transform = SidebarButton.TransformToVisual(null);
            bounds = transform.TransformBounds(new Rect(0, 0,
                                                        SidebarButton.ActualWidth,
                                                        SidebarButton.ActualHeight));
            Windows.Graphics.RectInt32 SidebarBtnRect = GetRect(bounds, scaleAdjustment);

            var rectArray = new Windows.Graphics.RectInt32[] { SearchBoxRect, SettingsBtnRect, SidebarBtnRect };

            InputNonClientPointerSource nonClientInputSrc =
                InputNonClientPointerSource.GetForWindowId(this.AppWindow.Id);
            nonClientInputSrc.SetRegionRects(NonClientRegionKind.Passthrough, rectArray);
        }

        private Windows.Graphics.RectInt32 GetRect(Rect bounds, double scale)
        {
            return new Windows.Graphics.RectInt32(
                _X: (int)Math.Round(bounds.X * scale),
                _Y: (int)Math.Round(bounds.Y * scale),
                _Width: (int)Math.Round(bounds.Width * scale),
                _Height: (int)Math.Round(bounds.Height * scale)
            );
        }

        private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            if (!AppSettings.Instance.DisableAllWeatherEffects)
            {
                if (args.WindowActivationState == WindowActivationState.Deactivated)
                {
                    TitleBarTextBlock.Foreground =
                        (SolidColorBrush)App.Current.Resources["WindowCaptionForegroundDisabled"];
                    Pikouna_Engine.ApplicationViewModel.Instance.CanPlayAnimations = false;
                }
                else
                {
                    TitleBarTextBlock.Foreground =
                        (SolidColorBrush)App.Current.Resources["WindowCaptionForeground"];
                    Pikouna_Engine.ApplicationViewModel.Instance.CanPlayAnimations = true;
                }
            }
        }

        private void Settings_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            AnimatedIcon.SetState(this.SettingsAnimatedIcon, "PointerOver");
        }

        private void Settings_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            AnimatedIcon.SetState(this.SettingsAnimatedIcon, "Normal");
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            var wrapperPage = ContentFrame.Content as WrapperPage;
            
            // reset all currently loaded weather states to load them in anew when the Pikouna Engine is reloaded
            Pikouna_Engine.WeatherViewModel.Instance.CloudCoverageExternal = 0;
            Pikouna_Engine.WeatherViewModel.Instance.Showers = 0;
            Pikouna_Engine.WeatherViewModel.Instance.WindSpeed = 0;
            Pikouna_Engine.WeatherViewModel.Instance.Snow = 0;
            Pikouna_Engine.WeatherViewModel.Instance.WeatherType = Pikouna_Engine.WeatherType.ClearSky;

            wrapperPage.showSettingsPage();
        }

        private void SidebarButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            AnimatedIcon.SetState(this.SidebarAnimatedIcon, "PointerOver");
        }

        private void SidebarButton_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            AnimatedIcon.SetState(this.SidebarAnimatedIcon, "Normal");
        }

        private void SidebarButton_Click(object sender, RoutedEventArgs e)
        {
            var wrapperPage = ContentFrame.Content as WrapperPage;
            wrapperPage.toggleSidebarState();
        }

        private List<InternalGeolocation> _suggestions = new();

        private async void TitleBarSearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                sender.ItemsSource = null;
                await Task.Delay(500);
                // only call the API when the text is still the same a second later to prevent bombarding with API calls while typing
                if (args.CheckCurrent())
                {
                    string query = sender.Text;

                    if (!string.IsNullOrWhiteSpace(query))
                    {
                        System.Diagnostics.Debug.WriteLine(query);
                        var suggestions = GeoLookup.SearchLocations(query);
                        _suggestions = suggestions;
                        sender.ItemsSource = new ObservableCollection<string>(
                            suggestions.ConvertAll(s => $"{s.name}, {s.state_code}, {s.country_code}") // Show display names
                        );

                        /*var suggestions = await GeoLookup.SearchLocationsAsync(query);

                        sender.ItemsSource = new ObservableCollection<string>(
                            suggestions.ConvertAll(s => s.DisplayName) // Show display names
                        );*/
                    }
                }
            }
        }

        private void TitleBarSearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            sender.ItemsSource = null;
            sender.Text = "";
            
            InternalGeolocation selectedLocation = null;

            if (args.ChosenSuggestion != null)
            {
                string chosen = args.ChosenSuggestion.ToString();
                selectedLocation = _suggestions.Find(
                    s => $"{s.name}, {s.state_code}, {s.country_code}" == chosen
                );
            }
            else if (!string.IsNullOrWhiteSpace(args.QueryText))
            {
                selectedLocation = _suggestions.Find(
                    s => s.name.Equals(args.QueryText, StringComparison.OrdinalIgnoreCase)
                );
            }

            if (selectedLocation != null)
            {
                System.Diagnostics.Debug.WriteLine($"Selected: {selectedLocation.name}, {selectedLocation.state_code}, {selectedLocation.country_code}");

                var wrapperPage = ContentFrame.Content as WrapperPage;
                wrapperPage.addLocation(selectedLocation, true);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("No matching location found.");
            }
        }
    }
}
