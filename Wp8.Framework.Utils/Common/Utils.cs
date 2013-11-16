using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;

namespace Wp8.Framework.Utils.Common
{
    public static class Utils
    {
        /// <summary>
        /// Method for removing the Back stack entry until a particular view. 
        /// </summary>
        /// <param name="type"></param>
        public static void RemoveBackStackUntil(Type type)
        {
            var frame = (Application.Current.RootVisual as PhoneApplicationFrame);
            if (frame != null)
            {
                var backStackList = frame.BackStack.ToList();
                if (backStackList.Any())
                {
                    if (frame.CanGoBack)
                    {
                        foreach (var entry in backStackList)
                        {
                            if (!entry.Source.ToString().Contains(String.Format("{0}.xaml", type.Name)))
                            {
                                frame.RemoveBackEntry();
                            }
                            else
                                return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Method to remove specific page from back stack
        /// </summary>
        /// <param name="type"></param>
        public static void RemoveSpecificPage(Type type)
        {
            var frame = (Application.Current.RootVisual as PhoneApplicationFrame);
            if (frame != null)
            {
                var backStackList = frame.BackStack.ToList();
                if (backStackList.Any())
                {
                    if (frame.CanGoBack)
                    {
                        if (backStackList.Any(entry => entry.Source.ToString().Contains(String.Format("{0}.xaml", type.Name))))
                        {
                            frame.RemoveBackEntry();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Method to create Thumbnail Image.
        /// </summary>
        /// <param name="stream">Image stream</param>
        /// <param name="width">thumbnail width</param>
        /// <param name="height">thumbnail height</param>
        /// <returns></returns>
        public static WriteableBitmap CreateThumbnail(Stream stream, int width, int height)
        {
            WriteableBitmap result = null;
            var waitHandle = new object();
            lock (waitHandle)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    lock (waitHandle)
                    {
                        var bi = new BitmapImage();
                        bi.SetSource(stream);
                        bi.CreateOptions = BitmapCreateOptions.None;

                        int w, h;
                        var ws = (double)width / bi.PixelWidth;
                        var hs = (double)height / bi.PixelHeight;
                        var scale = (ws > hs) ? ws : hs;
                        w = (int)(bi.PixelWidth * scale);
                        h = (int)(bi.PixelHeight * scale);

                        var im = new Image { Stretch = Stretch.UniformToFill, Source = bi };

                        result = new WriteableBitmap(width, height);
                        var tr = new CompositeTransform
                        {
                            CenterX = (ws > hs) ? 0 : (width - w) / 2,
                            CenterY = (ws < hs) ? 0 : (height - h) / 2,
                            ScaleX = scale,
                            ScaleY = scale
                        };
                        result.Render(im, tr);
                        result.Invalidate();
                        bi.UriSource = null;
                        Monitor.Pulse(waitHandle);
                    }

                });

                Monitor.Wait(waitHandle);
            }
            return result;
        }
    }
}
