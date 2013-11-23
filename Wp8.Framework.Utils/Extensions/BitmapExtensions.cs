using System;
using System.Windows.Media.Imaging;
using Wp8.Framework.Utils.Utils;

namespace Wp8.Framework.Utils.Extensions
{
    public static class BitmapExtensions
    {
        public static void FromByteArray(
            this WriteableBitmap writeableBitmap, byte[] buffer)
        {
            Buffer.BlockCopy(buffer, 0, writeableBitmap.Pixels, 0, buffer.Length);
        }

        public static byte[] ToByteArray(this WriteableBitmap writableBitmap)
        {
            ArgumentValidator.AssertNotNull(writableBitmap, "writableBitmap");
            var pixels = writableBitmap.Pixels;
            var arrayLength = pixels.Length * 4;
            var result = new byte[arrayLength];
            Buffer.BlockCopy(pixels, 0, result, 0, arrayLength);
            return result;
        }
    }
}
