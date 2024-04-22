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

        private int maxDroplets = 100;
        private double maxAnimationSeconds = 1.5; //3
        private double minAnimationSeconds = 0.5; //1

        Random random = new Random();

        public RainDrops()
        {
            this.InitializeComponent();
            this.Loaded += Rainy_Loaded;
        }

        private void Rainy_Loaded(object sender, RoutedEventArgs e)
        {
            startRain();
            UmbrellaSafeAreaFrame.Navigate(typeof(Conditions.Rendered.Sunny), null, null);
        }

        private async void startRain()
        {
            for (int i = 0; i < maxDroplets; i++)
            {
                createDroplet();

                await Task.Delay(10);
            }
        }

        private void createDroplet()
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

            setDropletAnimation(dropletImage);
        }

        private void setDropletPosition(Image dropletImage)
        {
            var translation = new System.Numerics.Vector3((float)random.Next(0, (int)RainGrid.ActualWidth), - maxSize, 0);
            dropletImage.Translation = translation;
        }

        private void setDropletAnimation(Image droplet)
        {
            var animationDuration = TimeSpan.FromSeconds(random.NextDouble() * maxAnimationSeconds);

            if (animationDuration < TimeSpan.FromSeconds(minAnimationSeconds))
                animationDuration = TimeSpan.FromMilliseconds(animationDuration.TotalMilliseconds + (minAnimationSeconds * 1000));

            Storyboard storyboard = new Storyboard();

            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.Duration = animationDuration;
            TranslateTransform translateTransform = new TranslateTransform();
            droplet.RenderTransform = translateTransform;
            Storyboard.SetTarget(doubleAnimation, droplet);
            Storyboard.SetTargetProperty(doubleAnimation, "UIElement.RenderTransform.(TranslateTransform.Y)");
            doubleAnimation.By = RainGrid.ActualHeight + maxSize;
            storyboard.Children.Add(doubleAnimation);

            droplet.Tag = Math.Round(animationDuration.TotalMilliseconds).ToString();

            storyboard.RepeatBehavior = RepeatBehavior.Forever;

            storyboard.Begin();
        }

        private async void startDetectingCollisions()
        {
            while (true)
            {
                await Task.Delay(50);
                await detectColision();
            }
        }

        private async Task detectColision()
        {
            System.Drawing.Point umbrellaCenter = Model.ViewModels.ViewRenderingModel.Instance.UmbrellaCenterPoint;
            int umbrellaRadius = Model.ViewModels.ViewRenderingModel.Instance.UmbrellaRadius;

            // Iterate through each droplet in the RainGrid
            foreach (var droplet in RainGrid.Children.OfType<Image>())
            {
                if (droplet.Translation.X > umbrellaCenter.X - umbrellaRadius && droplet.Translation.X < umbrellaCenter.X + umbrellaRadius)
                {
                    GeneralTransform transform = droplet.TransformToVisual(RainGrid);

                    Point position = transform.TransformPoint(new Point(0, 0));

                    // Calculate the distance from the droplet to the center of the umbrella
                    double dropletDistance = Math.Sqrt(
                        Math.Pow(position.X - umbrellaCenter.X, 2) +
                        Math.Pow(position.Y - umbrellaCenter.Y, 2));

                    // Check if the droplet is within the radius of the umbrella with a generous padding to account for delays
                    if (dropletDistance <= umbrellaRadius + 20)
                    {
                        // Hide the droplet
                        hideDroplet(droplet);
                    }
                }
            }
            return;
        }

        private async void hideDroplet(Image droplet)
        {
            droplet.Visibility = Visibility.Collapsed;

            GeneralTransform transform = droplet.TransformToVisual(RainGrid);
            double startingPosition = Convert.ToDouble(transform.TransformPoint(new Point(0, 0)).Y);

            string duration = droplet.Tag as string;
            double animationDuration = Convert.ToDouble(duration);

            double animationPercentage = 1 - ((startingPosition + 25) / (RainGrid.ActualHeight + 50));

            // artificially reduce the delay to account for inaccuracies
            await Task.Delay((int)Math.Round((double)(animationDuration * animationPercentage)) - 90);
            droplet.Visibility = Visibility.Visible;
        }

        private async void RainGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var starGridSize = RainGrid.ActualSize;
            await Task.Delay(50);
            if (starGridSize == RainGrid.ActualSize)
            {
                RainGrid.Children.Clear();
                startRain();
            }
        }
    }
}
