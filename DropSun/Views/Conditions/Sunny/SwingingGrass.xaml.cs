using DropSun.Model.ViewModels;
using Microsoft.UI.Composition;
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
using Windows.Foundation;
using Windows.Foundation.Collections;
using static System.Net.Mime.MediaTypeNames;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DropSun.Views.Conditions.Sunny
{
    public sealed partial class SwingingGrass : Page
    {
        private int[] GrassAnimationModifiers = new int[] { 0, 20, 5, 16, 4, 38, 20, 35, 10, 0 };
        private double coveredlength = -20;
        private double minAnimationSeconds = 0.75;
        private double maxAnimationSeconds = 1.5;
        Random random = new Random();

        public SwingingGrass()
        {
            this.InitializeComponent();
            this.Loaded += SwingingGrass_Loaded;
        }

        private void SwingingGrass_Loaded(object sender, RoutedEventArgs e)
        {
            if(ViewRenderingModel.Instance.WeatherCondition == Model.Weather.Condition.Sunny) loadGrass();
        }
        private void loadGrass()
        {
            while (coveredlength < GrassGrid.ActualWidth)
            {
                Microsoft.UI.Xaml.Controls.Image grassBlade = new();
                BitmapImage bitmapImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Application/WeatherObjects/GrassBlade.png"));
                grassBlade.Source = bitmapImageSource;

                double scaleFactor = 0.4 + (random.NextDouble() / 2);
                grassBlade.Height = 200 * scaleFactor;
                grassBlade.Width = 40 * scaleFactor;
                grassBlade.Stretch = Stretch.Fill;
                
                grassBlade.HorizontalAlignment = HorizontalAlignment.Left;
                grassBlade.VerticalAlignment = VerticalAlignment.Bottom;
                grassBlade.Translation = new System.Numerics.Vector3((float)coveredlength, 10, 0);

                coveredlength += grassBlade.ActualWidth * 0.5;
                GrassGrid.Children.Add(grassBlade);

                animateGrass(grassBlade);
            }
        }

        private void animateGrass(Microsoft.UI.Xaml.Controls.Image grass)
        {
            var animationDuration = TimeSpan.FromSeconds(random.NextDouble() * maxAnimationSeconds);

            if (animationDuration < TimeSpan.FromSeconds(minAnimationSeconds))
                animationDuration = TimeSpan.FromMilliseconds(animationDuration.TotalMilliseconds + (minAnimationSeconds * 1000));

            Storyboard storyboard = new Storyboard();

            RotateTransform rotateTransform = new RotateTransform()
            {
                CenterX = grass.Width/2,
                CenterY = grass.Height,
                Angle = 0
            };
            grass.RenderTransform = rotateTransform;

            DoubleAnimationUsingKeyFrames rotationKeyFrames = new DoubleAnimationUsingKeyFrames();
            rotationKeyFrames.Duration = animationDuration * 10;

            PowerEase easingFunction = new PowerEase { EasingMode = EasingMode.EaseInOut };

            for (int index = 1; index < GrassAnimationModifiers.Length; index++)
            {
                TimeSpan keyTime = TimeSpan.FromSeconds(index * animationDuration.TotalSeconds);

                EasingDoubleKeyFrame keyFrame = new EasingDoubleKeyFrame
                {
                    Value = GrassAnimationModifiers[index],
                    KeyTime = keyTime,
                    EasingFunction = easingFunction
                };

                rotationKeyFrames.KeyFrames.Add(keyFrame);
            }

            Storyboard.SetTarget(rotationKeyFrames, grass);
            Storyboard.SetTargetProperty(rotationKeyFrames, "(UIElement.RenderTransform).(RotateTransform.Angle)");

            storyboard.Children.Add(rotationKeyFrames);

            storyboard.RepeatBehavior = RepeatBehavior.Forever;
            storyboard.Begin();
        }
    }
}
