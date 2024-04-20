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

namespace DropSun.Views
{
    public sealed partial class WeatherView : Page
    {
        public WeatherView()
        {
            this.InitializeComponent();
            this.DataContext = Model.ViewModels.ViewRenderingModel.Instance;

            FrameNavigationOptions navOptions = new FrameNavigationOptions();
            Type pageType = typeof(Conditions.Rendered.Sunny);
            ContentFrame.NavigateToType(pageType, null, navOptions);
        }

        private void ContentGrid_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            // update the ViewModel with new 
            Model.ViewModels.ViewRenderingModel.Instance.CursorPosition = e.GetCurrentPoint(ContentGrid).Position;
        }
    }
}
