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
using Microsoft.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DropSun.Views.Conditions.Rainy
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RainDrops : Page
    {
        private int minSize = 10;
        private int maxSize = 20;

        private int maxDroplets = 150;
        private double maxAnimationSeconds = 3.0;
        private double minAnimationSeconds = 1.0;

        Random random = new Random();

        public RainDrops()
        {
            this.InitializeComponent();
            this.Loaded += Rainy_Loaded;
        }

        private void Rainy_Loaded(object sender, RoutedEventArgs e)
        {
            startRain();
        }

        private async void startRain()
        {
            for (int i = 0; i < maxDroplets; i++)
            {
                createDroplet();

                await Task.Delay(10);
            }
        }

        private async void createDroplet()
        {
            Image dropletImage = new Image();
            BitmapImage bitmapImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Application/WeatherObjects/Raindrop.png"));

            dropletImage.Source = bitmapImageSource;

            // Set a random size between minSize and maxSize
            double size = random.Next(minSize, maxSize);
            dropletImage.Width = size;
            dropletImage.Height = size;

            RainGrid.Children.Add(dropletImage);
            dropletImage.HorizontalAlignment = HorizontalAlignment.Left;
            dropletImage.VerticalAlignment = VerticalAlignment.Top;

            setDropletPosition(dropletImage);

            var animationDuration = setDropletAnimation(dropletImage);

            await Task.Delay(animationDuration);
            recycleDroplet(dropletImage);
        }

        private void setDropletPosition(Image dropletImage)
        {
            var translation = new System.Numerics.Vector3((float)random.Next(0, (int)RainGrid.ActualWidth), -100, 0);
            dropletImage.Translation = translation;
        }

        private TimeSpan setDropletAnimation(Image droplet)
        {
            droplet.TranslationTransition = new Vector3Transition();
            var animationDuration = TimeSpan.FromSeconds(random.NextDouble() * maxAnimationSeconds);
            if (animationDuration < TimeSpan.FromSeconds(minAnimationSeconds)) animationDuration = animationDuration = TimeSpan.FromMilliseconds(animationDuration.TotalMilliseconds + (minAnimationSeconds * 1000));
            droplet.TranslationTransition.Duration = animationDuration;

            droplet.Translation = new System.Numerics.Vector3((float)droplet.Translation.X, (float)RainGrid.ActualHeight + 350, 0);

            return animationDuration;
        }

        private async void recycleDroplet(Image droplet)
        {
            droplet.TranslationTransition = null;
            setDropletPosition(droplet);
            var animationDuration = setDropletAnimation(droplet);
            await Task.Delay(animationDuration);
            recycleDroplet(droplet);
        }
    }
}
