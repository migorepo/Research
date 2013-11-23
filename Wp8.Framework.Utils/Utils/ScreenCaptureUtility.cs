using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Xna.Framework.Media;

namespace Wp8.Framework.Utils.Utils
{
    public class ScreenCaptureUtility
    {
        public static void CaptureImageToMediaLibrary(FrameworkElement element, string fileName = null)
        {
            ArgumentValidator.AssertNotNull(element, "element");

            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = string.Format("Captured_{0:yyyy-MM-dd-HH-mm-ss}.jpg", DateTime.Now);
            }

            var width = (int)element.ActualWidth;
            var height = (int)element.ActualHeight;
            var bitmap = new WriteableBitmap(width, height);
            bitmap.Render(element, null);
            bitmap.Invalidate();

            var mediaLibrary = new MediaLibrary();

            using (var stream = new MemoryStream())
            {
                bitmap.SaveJpeg(stream, width, height, 0, 100);
                stream.Position = 0;
                mediaLibrary.SavePicture(fileName, stream);
            }
        }
    }
}
