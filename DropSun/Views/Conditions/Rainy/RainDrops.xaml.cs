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
using Microsoft.UI.Xaml.Media.Animation;
using System.Diagnostics;
using Microsoft.UI.Xaml.Documents;

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
        private int dropletsPerGrid = 10;
        private double maxAnimationSeconds = 3;
        private double minAnimationSeconds = 1;

        private int dropletsGridWidth = 400;
        private int dropletsGridHeight = 200;

        Random random = new Random();

        public RainDrops()
        {
            this.InitializeComponent();
            this.Loaded += Rainy_Loaded;
        }

        private void Rainy_Loaded(object sender, RoutedEventArgs e)
        {
            dropletsGridHeight = Convert.ToInt32(RainGrid.ActualHeight);
            dropletsGridWidth = Convert.ToInt32(RainGrid.ActualWidth);
            startRain();
            //UmbrellaSafeAreaFrame.Navigate(typeof(Conditions.Rendered.Sunny), null, null);
        }

        private async void startRain()
        {
            for (int i = 0; i < maxDroplets / dropletsPerGrid; i++)
            {
                createDroplet();

                await Task.Delay(50);
            }
        }

        private void createDroplet()
        {
            Grid dropletsGrid = new Grid();
            dropletsGrid.Height = dropletsGridHeight;
            dropletsGrid.Height = dropletsGridWidth;
            dropletsGrid.HorizontalAlignment = HorizontalAlignment.Left;
            dropletsGrid.VerticalAlignment = VerticalAlignment.Bottom;
            dropletsGrid.Translation = new System.Numerics.Vector3(0, (float) -(RainGrid.ActualHeight + dropletsGrid.ActualHeight + maxSize), 0);
            RainGrid.Children.Add(dropletsGrid);

            for (int i = 0; i < dropletsPerGrid; i++)
            {
                Image dropletImage = new Image();
                BitmapImage bitmapImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Application/WeatherObjects/Raindrop.png"));

                dropletImage.Source = bitmapImageSource;

                // Set a random size between minSize and maxSize
                double size = random.Next(minSize, maxSize);
                dropletImage.Width = size;
                dropletImage.Height = size;

                dropletsGrid.Children.Add(dropletImage);

                dropletImage.HorizontalAlignment = HorizontalAlignment.Left;
                dropletImage.VerticalAlignment = VerticalAlignment.Top;

                setDropletPosition(dropletImage, dropletsGridWidth, dropletsGridHeight);
            }

            setDropletAnimation(dropletsGrid, dropletsGridWidth, dropletsGridHeight);
        }

        private void setDropletPosition(Image dropletImage, double widthMax, double heightMax)
        {
            var translation = new System.Numerics.Vector3((float)random.Next(0, (int)widthMax), (float)random.Next(0, (int)heightMax), 0);
            dropletImage.Translation = translation;
        }

        private void setDropletAnimation(Grid droplets, double gridWidth, double gridHeight)
        {
            var animationDuration = TimeSpan.FromSeconds(random.NextDouble() * maxAnimationSeconds);

            if (animationDuration < TimeSpan.FromSeconds(minAnimationSeconds))
                animationDuration = TimeSpan.FromMilliseconds(animationDuration.TotalMilliseconds + (minAnimationSeconds * 1000));

            Storyboard storyboard = new Storyboard();

            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.Duration = animationDuration;
            TranslateTransform translateTransform = new TranslateTransform();
            droplets.RenderTransform = translateTransform;
            Storyboard.SetTarget(doubleAnimation, droplets);
            Storyboard.SetTargetProperty(doubleAnimation, "UIElement.RenderTransform.(TranslateTransform.Y)");
            doubleAnimation.By = RainGrid.ActualHeight + (dropletsGridHeight * 2);
            storyboard.Children.Add(doubleAnimation);

            storyboard.RepeatBehavior = RepeatBehavior.Forever;

            storyboard.Begin();
        }

        private async void RainGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var starGridSize = RainGrid.ActualSize;
            await Task.Delay(50);
            if (starGridSize == RainGrid.ActualSize)
            {
                dropletsGridHeight = Convert.ToInt32(RainGrid.ActualHeight);
                dropletsGridWidth = Convert.ToInt32(RainGrid.ActualWidth);
                RainGrid.Children.Clear();
                startRain();
            }
        }
    }
}
