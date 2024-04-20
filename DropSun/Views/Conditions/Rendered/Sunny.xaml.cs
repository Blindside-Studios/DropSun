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
using System.Xml.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DropSun.Views.Conditions.Rendered
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Sunny : Page
    {
        public Sunny()
        {
            this.InitializeComponent();
            this.DataContext = Model.ViewModels.ViewRenderingModel.Instance;

            FrameNavigationOptions navOptions = new FrameNavigationOptions();

            Type pageTypeSky = typeof(Conditions.Sunny.BlueSky);
            BlueSkyFrame.NavigateToType(pageTypeSky, null, navOptions);

            Type pageTypeSun = typeof(Conditions.Sunny.Sun);
            SunFrame.NavigateToType(pageTypeSun, null, navOptions);
            
            Type pageTypeGround = typeof(Conditions.Sunny.Ground);
            GroundFrame.NavigateToType(pageTypeGround, null, navOptions);
        }
    }
}
