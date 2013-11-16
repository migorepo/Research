using System.IO;
using System.IO.IsolatedStorage;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Wp8.Framework.Utils.Common
{
    public class IsoStoreImageSource : DependencyObject
    {
        public static void SetIsoStoreFileName(UIElement element, string value)
        {
            element.SetValue(IsoStoreFileNameProperty, value);
        }
        public static string GetIsoStoreFileName(UIElement element)
        {
            return (string)element.GetValue(IsoStoreFileNameProperty);
        }


        public static readonly DependencyProperty IsoStoreFileNameProperty =
            DependencyProperty.RegisterAttached("IsoStoreFileName", typeof(string), typeof(IsoStoreImageSource), new PropertyMetadata("", Changed));

        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var img = d as Image;

            if (img != null)
            {
                var path = e.NewValue as string;
                var uiThread = SynchronizationContext.Current;

                Task.Factory.StartNew(() =>
                {
                    using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        if (path != null && isoStore.FileExists(path))
                        {
                            var stream = isoStore.OpenFile(path, FileMode.Open, FileAccess.Read);
                            uiThread.Post(_ =>
                            {
                                var image = new BitmapImage();
                                image.SetSource(stream);
                                img.Source = image;
                            }, null);
                        }
                    }
                });
            }
        }
    }
}
