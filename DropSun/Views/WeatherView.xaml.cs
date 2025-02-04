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
            WeatherState.Instance.PropertyChanged += Weather_PropertyChanged;
        }

        private void WeatherView_Loaded(object sender, RoutedEventArgs e)
        {
            ContentFrame.NavigateToType(typeof(Pikouna_Engine.WeatherView), null, null);
            GeneralFrame.NavigateToType(typeof(WeatherCards.General), null, null);

            WeatherCards = new ObservableCollection<WeatherCard>
            {
                new WeatherCard { PageType = typeof(BlankCard), ColumnSpan = 1, RowSpan = 1, Width = 300, Height = 300 },
                new WeatherCard { PageType = typeof(BlankCard), ColumnSpan = 2, RowSpan = 1, Width = 620, Height = 300 },
                new WeatherCard { PageType = typeof(BlankCard), ColumnSpan = 1, RowSpan = 1, Width = 300, Height = 300 },
                new WeatherCard { PageType = typeof(BlankCard), ColumnSpan = 1, RowSpan = 1, Width = 300, Height = 300 }
            };
            WidgetSpawner.ItemsSource = WeatherCards;
        }

        private void Weather_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // use this later to update the properties in the view, unless I expand this into another view model, at which point this can be accessed throughout
        }

        private void ContentGrid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Pikouna_Engine.OzoraViewModel.Instance.MouseEngaged = true;
        }

        private void ContentGrid_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Pikouna_Engine.OzoraViewModel.Instance.MouseEngaged = false;
        }

        private void ContentGrid_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            Pikouna_Engine.OzoraViewModel.Instance.MousePosition = e.GetCurrentPoint(ContentGrid).Position;
        }
    }
}
