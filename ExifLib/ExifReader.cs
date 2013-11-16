using System;
using System.IO;

namespace ExifLib
{

    public class ExifReader
    {
        private bool littleEndian;

        public JpegInfo info
        {
            get;
            private set;
        }

        protected ExifReader(Stream stream)
        {
            this.info = new JpegInfo();
            int num = stream.ReadByte();
            if (num != 255 || stream.ReadByte() != 216)
            {
                return;
            }
            this.info.IsValid = true;
            while (true)
            {
                int num1 = 0;
                int num2 = 0;
                num = 0;
                while (true)
                {
                    num1 = stream.ReadByte();
                    if (num1 != 255 && num2 == 255)
                    {
                        break;
                    }
                    num2 = num1;
                    num++;
                }
                int num3 = stream.ReadByte();
                int num4 = stream.ReadByte();
                int num5 = num3 << 8 | num4;
                byte[] numArray = new byte[num5];
                numArray[0] = (byte)num3;
                numArray[1] = (byte)num4;
                int num6 = stream.Read(numArray, 2, num5 - 2);
                if (num6 != num5 - 2)
                {
                    return;
                }
                int num7 = num1;
                switch (num7)
                {
                    case 192:
                    case 193:
                    case 194:
                    case 195:
                    case 197:
                    case 198:
                    case 199:
                    case 201:
                    case 202:
                    case 203:
                    case 205:
                    case 206:
                    case 207:
                        {
                            this.ProcessSOF(numArray, num1);
                            goto case 216;
                        }
                    case 196:
                    case 200:
                    case 204:
                    case 208:
                    case 209:
                    case 210:
                    case 211:
                    case 212:
                    case 213:
                    case 214:
                    case 215:
                    case 216:
                        {
                            numArray = null;
                            GC.Collect();
                            continue;
                        }
                    case 217:
                    case 218:
                        {
                            break;
                        }
                    default:
                        {
                            if (num7 == 225)
                            {
                                if (numArray[2] != 69 || numArray[3] != 120 || numArray[4] != 105 || numArray[5] != 102)
                                {
                                    goto case 216;
                                }
                                this.ProcessExif(numArray);
                                goto case 216;
                            }
                            else
                            {
                                if (num7 == 237)
                                {
                                    goto case 216;
                                }
                                goto case 216;
                            }
                        }
                }
            }
        }

        private int DirOffset(int start, int num)
        {
            return start + 2 + 12 * num;
        }

        private void ProcessExif(byte[] section)
        {
            int num = 6;
            int num1 = num;
            num = num1 + 1;
            if (section[num1] == 0)
            {
                int num2 = num;
                num = num2 + 1;
                if (section[num2] == 0)
                {
                    if (section[num] != 73 || section[num + 1] != 73)
                    {
                        if (section[num] != 77 || section[num + 1] != 77)
                        {
                            return;
                        }
                        this.littleEndian = false;
                    }
                    else
                    {
                        this.littleEndian = true;
                    }
                    num = num + 2;
                    int num3 = ExifIO.ReadUShort(section, num, this.littleEndian);
                    num = num + 2;
                    if (num3 != 42)
                    {
                        return;
                    }
                    num3 = ExifIO.ReadInt(section, num, this.littleEndian);
                    num = num + 4;
                    if ((num3 < 8 || num3 > 16) && (num3 < 16 || num3 > (int)section.Length - 16))
                    {
                        return;
                    }
                    this.ProcessExifDir(section, num3 + 8, 8, (int)section.Length - 8, 0, ExifIFD.Exif);
                    return;
                }
            }
        }

        private void ProcessExifDir(byte[] section, int offsetDir, int offsetBase, int length, int depth, ExifIFD ifd)
        {
            if (depth > 4)
            {
                return;
            }
            ushort num = ExifIO.ReadUShort(section, offsetDir, this.littleEndian);
            if (offsetDir + 2 + 12 * num >= offsetDir + length)
            {
                return;
            }
            int num1 = 0;
            for (int i = 0; i < num; i++)
            {
                num1 = this.DirOffset(offsetDir, i);
                ExifTag exifTag = new ExifTag(section, num1, offsetBase, length, this.littleEndian);
                if (exifTag.IsValid)
                {
                    int tag = exifTag.Tag;
                    if (tag == 34665)
                    {
                        int num2 = offsetBase + exifTag.GetInt(0);
                        if (num2 <= offsetBase + length)
                        {
                            this.ProcessExifDir(section, num2, offsetBase, length, depth + 1, ExifIFD.Exif);
                        }
                    }
                    else
                    {
                        if (tag == 34853)
                        {
                            int num3 = offsetBase + exifTag.GetInt(0);
                            if (num3 <= offsetBase + length)
                            {
                                this.ProcessExifDir(section, num3, offsetBase, length, depth + 1, ExifIFD.Gps);
                            }
                        }
                        else
                        {
                            exifTag.Populate(this.info, ifd);
                        }
                    }
                }
            }
            num1 = this.DirOffset(offsetDir, (int)num) + 4;
            if (num1 <= offsetBase + length)
            {
                num1 = ExifIO.ReadInt(section, offsetDir + 2 + 12 * num, this.littleEndian);
                if (num1 > 0)
                {
                    int num4 = offsetBase + num1;
                    if (num4 <= offsetBase + length && num4 >= offsetBase)
                    {
                        this.ProcessExifDir(section, num4, offsetBase, length, depth + 1, ifd);
                    }
                }
            }
            if (this.info.ThumbnailData == null && this.info.ThumbnailOffset > 0 && this.info.ThumbnailSize > 0)
            {
                this.info.ThumbnailData = new byte[this.info.ThumbnailSize];
                Array.Copy(section, offsetBase + this.info.ThumbnailOffset, this.info.ThumbnailData, 0, this.info.ThumbnailSize);
            }
        }

        private void ProcessSOF(byte[] section, int marker)
        {
            this.info.Height = section[3] << 8 | section[4];
            this.info.Width = section[5] << 8 | section[6];
            int num = section[7];
            this.info.IsColor = num == 3;
        }

        public static JpegInfo ReadJpeg(FileInfo fi)
        {
            JpegInfo jpegInfo;
            DateTime now = DateTime.Now;
            FileStream fileStream = fi.OpenRead();
            using (fileStream)
            {
                ExifReader exifReader = new ExifReader(fileStream);
                exifReader.info.FileSize = (int)fi.Length;
                exifReader.info.FileName = fi.Name;
                exifReader.info.LoadTime = DateTime.Now - now;
                jpegInfo = exifReader.info;
            }
            return jpegInfo;
        }

        public static JpegInfo ReadJpeg(Stream FileStream)
        {
            DateTime now = DateTime.Now;
            ExifReader exifReader = new ExifReader(FileStream);
            exifReader.info.FileSize = (int)FileStream.Length;
            exifReader.info.LoadTime = DateTime.Now - now;
            return exifReader.info;
        }
    }
}
