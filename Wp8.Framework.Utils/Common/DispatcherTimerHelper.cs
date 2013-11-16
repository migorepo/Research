using System;
using System.Windows;
using System.Windows.Threading;

namespace Wp8.Framework.Utils.Common
{
    public static class DispatcherTimerHelper
    {
        public static void On(TimeSpan fromSeconds, Action action)
        {
            var t = new DispatcherTimer { Interval = fromSeconds };
            t.Tick += (s, args) =>
            {
                action();
                t.Stop();
            };
            t.Start();

        }


        public static void OnWithDispatcher(TimeSpan fromSeconds, Action action)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                var t = new DispatcherTimer { Interval = fromSeconds };
                t.Tick += (s, args) =>
                {
                    action();
                    t.Stop();
                };
                t.Start();
            });
        }
    }
}
