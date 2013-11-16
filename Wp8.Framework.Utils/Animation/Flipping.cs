using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Wp8.Framework.Utils.Animation
{
    public static class Flipping
    {
        public static void FlipToBack(UIElement parentCtrl, UIElement frontCtrl, UIElement backCtrl, TimeSpan duration,
                                      Action completed = null)
        {
            Flip(parentCtrl, frontCtrl, backCtrl, duration, true, completed);
        }

        public static void FlipToFront(UIElement parentCtrl, UIElement frontCtrl, UIElement backCtrl, TimeSpan duration,
                                       Action completed = null)
        {
            Flip(parentCtrl, frontCtrl, backCtrl, duration, false, completed);
        }

        public static void Flip(UIElement parentCtrl, UIElement frontCtrl, UIElement backCtrl, TimeSpan duration,
                                bool transitionToBack, Action completed = null)
        {
            duration = new TimeSpan(duration.Ticks/2);

            if (!(parentCtrl.Projection is PlaneProjection))
                parentCtrl.Projection = new PlaneProjection();

            var animation = new DoubleAnimation
                {
                    From = 0.0,
                    To = 90.0*(transitionToBack ? 1 : -1),
                    Duration = new Duration(duration)
                };

            var story = new Storyboard();
            story.Children.Add(animation);

            Storyboard.SetTarget(animation, parentCtrl.Projection);
            Storyboard.SetTargetProperty(animation, new PropertyPath("RotationY"));
            story.Completed += delegate
                {
                    animation = new DoubleAnimation
                        {
                            From = 270.0*(transitionToBack ? 1 : -1),
                            To = 360.0*(transitionToBack ? 1 : -1),
                            Duration = new Duration(duration)
                        };

                    story = new Storyboard();
                    story.Children.Add(animation);

                    Storyboard.SetTarget(animation, parentCtrl.Projection);
                    Storyboard.SetTargetProperty(animation, new PropertyPath("RotationY"));
                    frontCtrl.Visibility = transitionToBack ? Visibility.Collapsed : Visibility.Visible;
                    backCtrl.Visibility = !transitionToBack ? Visibility.Collapsed : Visibility.Visible;

                    story.Completed += delegate
                        {
                            ((PlaneProjection) parentCtrl.Projection).RotationY = 0.0;
                            if (completed != null)
                                completed();
                        };
                    story.Begin();
                };
            story.Begin();
        }
    }
}
