using System.Windows.Media.Imaging;

namespace Wp8.Framework.Utils.Common
{
    public static class BitmapMemoryCleanUpHelper
    {
        public static byte[] EmptyImage = new byte[] { 71, 73, 70, 56, 57, 97, 1, 0, 1, 0, 128, 0, 0, 255, 255, 255, 0, 0, 0, 33, 249, 4, 1, 0, 0, 0, 0, 44, 0, 0, 0, 0, 1, 0, 1, 0, 0, 2, 2, 68, 1, 0, 59 };

        public static void DisposeImage(BitmapImage img)
        {
            if (img != null) img.SetSource(new System.IO.MemoryStream(EmptyImage));
        }

        public static void DisposeImage(WriteableBitmap img)
        {
            if (img != null) img.SetSource(new System.IO.MemoryStream(EmptyImage));
        }
    }
}
