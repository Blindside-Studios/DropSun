using DropSun.Model.ViewModels;
using DropSun.Views.WeatherCards;
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
using Pikouna_Engine;
using System.Collections.ObjectModel;
using ShinGrid;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DropSun.Views
{
    public class WeatherCard
    {
        public Type PageType { get; set; }
        public int ColumnSpan { get; set; }
        public int RowSpan { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public sealed partial class WeatherView : Page
    {
        public ObservableCollection<WeatherCard> WeatherCards { get; set; }


        public WeatherView()
        {
            this.InitializeComponent();
            this.Loaded += WeatherView_Loaded;
        }

        private void WeatherView_Loaded(object sender, RoutedEventArgs e)
        {
            ContentFrame.NavigateToType(typeof(Pikouna_Engine.WeatherView), null, null);
            updatePusherSize();
            loadGridWeatherCards();
        }

        private void Instance_HeightChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ShinGridContainerFrame.Height = ShinGridViewModel.Instance.FinalHeight;
        }

        private void loadGridWeatherCards()
        {
            ShinGridViewModel.Instance.ColumnWidth = 200;
            ShinGridViewModel.Instance.RowHeight = 200;
            ShinGridViewModel.Instance.CornerRadius = 8;
            // Only override if it hasn't been overridden before, otherwise this will crash!
            if (ShinGridViewModel.Instance.PanelInstances == null) ShinGridViewModel.Instance.PanelInstances = new List<PanelInstance>()
            {
                new PanelInstance { PageType = typeof(OverviewCard), Index = 0, ColumnSpan = 2 },
                new PanelInstance { PageType = typeof(ForecastCard), Index = 1, ColumnSpan = 3 },
                new PanelInstance { PageType = typeof(BlankCard), Index = 2 },
                new PanelInstance { PageType = typeof(BlankCard), Index = 3 },
                new PanelInstance { PageType = typeof(BlankCard), Index = 4 },
                new PanelInstance { PageType = typeof(BlankCard), Index = 5 },
            };
            ShinGridContainerFrame.NavigateToType(typeof(ShinGrid.ShinGrid), null, null);
            ShinGridViewModel.Instance.HeightChanged += Instance_HeightChanged;
        }

        private void ContentGrid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (AppSettings.Instance.InteractionStyle != SunInteractionStyle.None) Pikouna_Engine.OzoraViewModel.Instance.MouseEngaged = true;
        }

        private void ContentGrid_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (AppSettings.Instance.InteractionStyle != SunInteractionStyle.None) Pikouna_Engine.OzoraViewModel.Instance.MouseEngaged = false;
        }

        private void ContentGrid_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (AppSettings.Instance.InteractionStyle != SunInteractionStyle.None) Pikouna_Engine.OzoraViewModel.Instance.MousePosition = e.GetCurrentPoint(ContentGrid).Position;
        }

        private void ContentGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            updatePusherSize();
        }

        private void updatePusherSize()
        {
            ContentTopBorder.Height = ContentGrid.ActualHeight - 210;
        }
    }
}
