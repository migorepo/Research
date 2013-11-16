using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Wp8.Framework.Utils.Animation
{
    public static class Fading
    {
        public static void FadeIn(UIElement obj, int duration, Action completed = null)
        {
            FadeIn(obj, new TimeSpan(0, 0, 0, 0, duration), completed);
        }

        public static void FadeIn(UIElement obj, TimeSpan duration, Action completed = null)
        {
            var animation = new DoubleAnimation
            {
                From = obj.Opacity,
                To = 1.0,
                EasingFunction = new ExponentialEase { Exponent = 6, EasingMode = EasingMode.EaseOut },
                Duration = new Duration(duration)
            };

            var story = new Storyboard();
            story.Children.Add(animation);

            Storyboard.SetTarget(animation, obj);
            Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));
            if (completed != null)
                story.Completed += delegate { completed(); };
            story.Begin();
        }

        public static void FadeOut(UIElement obj, int duration, Action completed = null)
        {
            FadeOut(obj, new TimeSpan(0, 0, 0, 0, duration), completed);
        }

        public static void FadeOut(UIElement obj, TimeSpan duration, Action completed = null)
        {
            var animation = new DoubleAnimation
            {
                From = obj.Opacity,
                To = 0.0,
                EasingFunction = new ExponentialEase { Exponent = 6, EasingMode = EasingMode.EaseOut },
                Duration = new Duration(duration)
            };

            var story = new Storyboard();
            story.Children.Add(animation);

            Storyboard.SetTarget(animation, obj);
            Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));

            if (completed != null)
                story.Completed += delegate { completed(); };
            story.Begin();
        }

        public static bool FadeInBackground(Panel panel, Uri backgroundSourceUri, double opacity,
        int msecs, Action completed = null)
        {
            if (backgroundSourceUri == null)
                return false;

            var brush = new ImageBrush();
            var image = new BitmapImage { UriSource = backgroundSourceUri };
            brush.Opacity = panel.Background != null ? panel.Background.Opacity : 0.0;
            brush.Stretch = Stretch.UniformToFill;
            brush.ImageSource = image;

            panel.Background = brush;
            brush.ImageOpened += delegate
            {
                var animation = new DoubleAnimation
                {
                    From = panel.Background != null ? panel.Background.Opacity : 0.0,
                    To = opacity,
                    Duration = new Duration(new TimeSpan(0, 0, 0, 0, msecs))
                };

                var story = new Storyboard();
                story.Children.Add(animation);

                Storyboard.SetTarget(animation, brush);
                Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));

                if (completed != null)
                    story.Completed += delegate { completed(); };
                story.Begin();
            };
            return true;
        }

        public static bool FadeOutBackground(Panel panel, int msecs, Action completed = null)
        {
            if (panel.Background == null || panel.Background.Opacity == 0.0)
                return false;

            var animation = new DoubleAnimation
            {
                From = panel.Background.Opacity,
                To = 0.0,
                Duration = new Duration(new TimeSpan(0, 0, 0, 0, msecs))
            };

            var story = new Storyboard();
            story.Children.Add(animation);

            Storyboard.SetTarget(animation, panel.Background);
            Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));

            story.Completed += delegate
            {
                panel.Background = null;
                if (completed != null)
                    completed();
            };

            story.Begin();
            return true;
        }
    }
}
