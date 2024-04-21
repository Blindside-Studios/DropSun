using DropSun.Views.Conditions.Rainy;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DropSun.Views.Conditions.Rendered
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Rainy : Page
    {
        public Rainy()
        {
            this.InitializeComponent();
            this.Loaded += Rainy_Loaded;
        }

        private void Rainy_Loaded(object sender, RoutedEventArgs e)
        {
            FrameNavigationOptions navOptions = new FrameNavigationOptions();
            Type pageType = typeof(RainDrops);
            DropletsFrame.NavigateToType(pageType, null, navOptions);

            UmbrellaImage.CenterPoint = new System.Numerics.Vector3(120, 200, 0);
        }
    }
}
