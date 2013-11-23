using System.IO;
using Wp8.Framework.Utils.Utils;

namespace Wp8.Framework.Utils.Extensions
{
    public static class StreamExtensions
    {
        public static void CopyStreamBytes(
            this Stream fromStream, Stream toStream, bool closeToStream = true)
        {
            ArgumentValidator.AssertNotNull(fromStream, "fromStream");
            ArgumentValidator.AssertNotNull(toStream, "toStream");

            if (toStream.CanWrite)
            {
                var fileBytes = ReadAllBytes(fromStream);
                toStream.Write(fileBytes, 0, fileBytes.Length);
                if (closeToStream)
                {
                    toStream.Close();
                }
            }
        }

        public static byte[] ReadAllBytes(this Stream fileStream)
        {
            ArgumentValidator.AssertNotNull(fileStream, "fileStream");

            /* Read the source file into a byte array. */
            var bytes = new byte[fileStream.Length];
            var readLength = (int)fileStream.Length;
            var bytesRead = 0;
            while (readLength > 0)
            {
                /* Read may return anything from 0 to readLength. */
                int read = fileStream.Read(bytes, bytesRead, readLength);

                /* When no bytes left to read it is the end of the file. */
                if (read == 0)
                {
                    break;
                }

                bytesRead += read;
                readLength -= read;
            }
            return bytes;
        }
    }
}
