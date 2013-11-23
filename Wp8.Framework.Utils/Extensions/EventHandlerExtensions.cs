using System;
using System.ComponentModel;
using Windows.Foundation;

namespace Wp8.Framework.Utils.Extensions
{
    public static class EventHandlerExtensions
    {
        public static void Raise<TEventArgs>(
            this EventHandler<TEventArgs> eventHandler, object sender, TEventArgs e)
            where TEventArgs : EventArgs
        {
            var tempEvent = eventHandler;
            if (tempEvent != null)
            {
                eventHandler(sender, e);
            }
        }

        public static void Raise(this PropertyChangedEventHandler eventHandler,
            object sender, PropertyChangedEventArgs e)
        {
            var tempEvent = eventHandler;
            if (tempEvent != null)
            {
                eventHandler(sender, e);
            }
        }

        public static void Raise(this PropertyChangedEventHandler eventHandler,
            object sender, string propertyName)
        {
            var tempEvent = eventHandler;
            if (tempEvent != null)
            {
                eventHandler(sender, new PropertyChangedEventArgs(propertyName));
            }
        }

        public static void Raise<TSender, TResult>(this TypedEventHandler<TSender, TResult> eventHandler,
            TSender sender, TResult e)
        {
            var tempEvent = eventHandler;
            if (tempEvent != null)
            {
                eventHandler(sender, e);
            }
        }
    }
}
