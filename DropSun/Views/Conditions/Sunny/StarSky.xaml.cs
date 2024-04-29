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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Newtonsoft.Json;
using Windows.Services.Store;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DropSun.Views.Conditions.Sunny
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StarSky : Page
    {
        int starsAmount = 500;
        int minSize = 3;
        int maxSize = 7;
        double starAnimationModifier = 0.33;

        private double gridWidthInitial = 0;
        private double gridHeightInitial = 0;

        public StarSky()
        {
            this.InitializeComponent();
            this.Loaded += StarSky_Loaded;
            gridWidthInitial = StarGrid.ActualWidth;
            gridHeightInitial = StarGrid.ActualHeight;
        }

        private void StarSky_Loaded(object sender, RoutedEventArgs e)
        {
            if (Model.ViewModels.ViewRenderingModel.Instance.WeatherCondition == Model.Weather.Condition.Sunny) generateStars();
        }

        private void generateStars()
        {
            Random random = new Random();

            for (int i =  0; i < this.starsAmount; i++)
            {
                Image starImage = new Image();
                BitmapImage bitmapImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Application/WeatherObjects/Star.png"));

                starImage.Source = bitmapImageSource;

                // Set a random size between minSize and maxSize
                double size = random.Next(minSize, maxSize);
                starImage.Width = size;
                starImage.Height = size;


                // run an animation on the biggest stars
                if (size >= minSize + ((1 - starAnimationModifier) * (maxSize - minSize)))
                {
                    int addGrid = (int)Math.Round(random.NextDouble() * 4);
                    Debug.WriteLine(addGrid);
                    switch (addGrid)
                    {
                        case 0:
                            AnimationsGrid1.Children.Add(starImage);
                            break;
                        case 1:
                            AnimationsGrid2.Children.Add(starImage);
                            break;
                        case 2:
                            AnimationsGrid3.Children.Add(starImage);
                            break;
                        case 3:
                            AnimationsGrid4.Children.Add(starImage);
                            break;
                        case 4:
                            AnimationsGrid5.Children.Add(starImage);
                            break;
                        default:
                            Debug.WriteLine("Not added to a grid");
                            break;
                    }
                }
                // render smaller stars at a lower opacity to create the illusion of distance
                else
                {
                    StarGrid.Children.Add(starImage);
                    starImage.Opacity = (random.NextDouble() / 2) + 0.4;
                }


                starImage.HorizontalAlignment = HorizontalAlignment.Left;
                starImage.VerticalAlignment = VerticalAlignment.Top;
                var translation = new System.Numerics.Vector3((float)random.Next(0, (int)StarGrid.ActualWidth), (float)random.Next(0, (int)StarGrid.ActualHeight), 0);
                starImage.Translation = translation;
            }

            startGridAnimation(AnimationsGrid1);
            startGridAnimation(AnimationsGrid2);
            startGridAnimation(AnimationsGrid3);
            startGridAnimation(AnimationsGrid4);
            startGridAnimation(AnimationsGrid5);
        }

        private async void StarGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (Grid grid in AllGrids.Children.OfType<Grid>())
            {
                foreach (Image star in grid.Children.OfType<Image>())
                {
                    star.Translation = new System.Numerics.Vector3(
                        star.Translation.X / (float)gridWidthInitial * (float)StarGrid.ActualWidth,
                        star.Translation.Y / (float)gridHeightInitial * (float)StarGrid.ActualHeight,
                        0);
                }
            }
            gridWidthInitial = StarGrid.ActualWidth;
            gridHeightInitial = StarGrid.ActualHeight;
        }

        private void startGridAnimation(Grid grid)
        {
            Random random = new Random();

            double animationDuration = 0;
            do animationDuration = random.NextDouble() * 2;
            while (animationDuration < 0.5);
            DoubleAnimation fadeAnimation = new DoubleAnimation
            {
                From = random.NextDouble() / 3 + 0.66,
                To = random.NextDouble() / 3,
                Duration = new Duration(TimeSpan.FromSeconds(animationDuration)),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };
            Storyboard.SetTarget(fadeAnimation, grid);
            Storyboard.SetTargetProperty(fadeAnimation, "Opacity");

            Storyboard starStoryboard = new Storyboard();
            starStoryboard.Children.Add(fadeAnimation);
            starStoryboard.Begin();
        }
    }
}
