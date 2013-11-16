using System;
using System.Text;

namespace ExifLib
{
    public class ExifTag
    {
        private static int[] BytesPerFormat;

        public int Components
        {
            get;
            private set;
        }

        public byte[] Data
        {
            get;
            private set;
        }

        public ExifTagFormat Format
        {
            get;
            private set;
        }

        public bool IsNumeric
        {
            get
            {
                ExifTagFormat format = this.Format;
                if (format != ExifTagFormat.STRING && format != ExifTagFormat.UNDEFINED)
                {
                    return true;
                }
                return false;
            }
        }

        public bool IsValid
        {
            get;
            private set;
        }

        public bool LittleEndian
        {
            get;
            private set;
        }

        public int Tag
        {
            get;
            private set;
        }

        static ExifTag()
        {
            int[] numArray = new int[] { 0, 1, 1, 2, 4, 8, 1, 1, 2, 4, 8, 4, 8 };
            ExifTag.BytesPerFormat = numArray;
        }

        public ExifTag(byte[] section, int sectionOffset, int offsetBase, int length, bool littleEndian)
        {
            this.IsValid = false;
            this.Tag = ExifIO.ReadUShort(section, sectionOffset, littleEndian);
            int num = ExifIO.ReadUShort(section, sectionOffset + 2, littleEndian);
            if (num < 1 || num > 12)
            {
                return;
            }
            this.Format = (ExifTagFormat)num;
            this.Components = ExifIO.ReadInt(section, sectionOffset + 4, littleEndian);
            if (this.Components > 65536)
            {
                return;
            }
            this.LittleEndian = littleEndian;
            int components = this.Components * ExifTag.BytesPerFormat[num];
            int num1 = 0;
            if (components <= 4)
            {
                num1 = sectionOffset + 8;
            }
            else
            {
                int num2 = ExifIO.ReadInt(section, sectionOffset + 8, littleEndian);
                if (num2 + components > length)
                {
                    return;
                }
                num1 = offsetBase + num2;
            }
            this.Data = new byte[components];
            Array.Copy(section, num1, this.Data, 0, components);
            this.IsValid = true;
        }

        public int GetInt(int componentIndex)
        {
            return (int)this.GetNumericValue(componentIndex);
        }

        public double GetNumericValue(int componentIndex)
        {
            ExifTagFormat format = this.Format;
            switch (format)
            {
                case ExifTagFormat.BYTE:
                    {
                        return (double)this.Data[componentIndex];
                    }
                case ExifTagFormat.STRING:
                case ExifTagFormat.UNDEFINED:
                    {
                        return 0;
                    }
                case ExifTagFormat.USHORT:
                    {
                        return (double)this.ReadUShort(componentIndex * 2);
                    }
                case ExifTagFormat.ULONG:
                    {
                        return (double)((float)this.ReadUInt(componentIndex * 4));
                    }
                case ExifTagFormat.URATIONAL:
                    {
                        return (double)((float)this.ReadUInt(componentIndex * 8)) / (double)((float)this.ReadUInt(componentIndex * 8 + 4));
                    }
                case ExifTagFormat.SBYTE:
                    {
                        return (double)((sbyte)this.Data[componentIndex]);
                    }
                case ExifTagFormat.SSHORT:
                    {
                        return (double)this.ReadShort(componentIndex * 2);
                    }
                case ExifTagFormat.SLONG:
                    {
                        return (double)this.ReadInt(componentIndex * 4);
                    }
                case ExifTagFormat.SRATIONAL:
                    {
                        return (double)this.ReadInt(componentIndex * 8) / (double)this.ReadInt(componentIndex * 8 + 4);
                    }
                case ExifTagFormat.SINGLE:
                    {
                        return (double)this.ReadSingle(componentIndex * 4);
                    }
                case ExifTagFormat.DOUBLE:
                    {
                        return this.ReadDouble(componentIndex * 8);
                    }
                default:
                    {
                        return 0;
                    }
            }
        }

        public string GetStringValue()
        {
            return this.GetStringValue(0);
        }

        public string GetStringValue(int componentIndex)
        {
            double numericValue;
            ExifTagFormat format = this.Format;
            if (format != ExifTagFormat.STRING)
            {
                switch (format)
                {
                    case ExifTagFormat.URATIONAL:
                        {
                            uint num = this.ReadUInt(componentIndex * 8);
                            uint num1 = this.ReadUInt(componentIndex * 8 + 4);
                            return string.Concat(num.ToString(), "/", num1.ToString());
                        }
                    case ExifTagFormat.SBYTE:
                        {
                            numericValue = this.GetNumericValue(componentIndex);
                            return numericValue.ToString();
                        }
                    case ExifTagFormat.UNDEFINED:
                        {
                            break;
                        }
                    default:
                        {
                            if (format == ExifTagFormat.SRATIONAL)
                            {
                                int num2 = this.ReadInt(componentIndex * 8);
                                int num3 = this.ReadInt(componentIndex * 8 + 4);
                                return string.Concat(num2.ToString(), "/", num3.ToString());
                            }
                            numericValue = this.GetNumericValue(componentIndex);
                            return numericValue.ToString();
                        }
                }
            }
            char[] chrArray = new char[] { ' ', '\t', '\r', '\n', default(char) };
            return Encoding.UTF8.GetString(this.Data, 0, (int)this.Data.Length).Trim(chrArray);
        }

        public virtual void Populate(JpegInfo info, ExifIFD ifd)
        {
            ExifId tag;
            if (ifd != ExifIFD.Exif)
            {
                if (ifd == ExifIFD.Gps)
                {
                    ExifGps exifGp = (ExifGps)this.Tag;
                    switch (exifGp)
                    {
                        case ExifGps.LatitudeRef:
                            {
                                if (this.GetStringValue() == "N")
                                {
                                    info.GpsLatitudeRef = ExifGpsLatitudeRef.North;
                                    return;
                                }
                                if (this.GetStringValue() != "S")
                                {
                                    break;
                                }
                                info.GpsLatitudeRef = ExifGpsLatitudeRef.South;
                                return;
                            }
                        case ExifGps.Latitude:
                            {
                                if (this.Components != 3)
                                {
                                    break;
                                }
                                info.GpsLatitude[0] = this.GetNumericValue(0);
                                info.GpsLatitude[1] = this.GetNumericValue(1);
                                info.GpsLatitude[2] = this.GetNumericValue(2);
                                return;
                            }
                        case ExifGps.LongitudeRef:
                            {
                                if (this.GetStringValue() == "E")
                                {
                                    info.GpsLongitudeRef = ExifGpsLongitudeRef.East;
                                    return;
                                }
                                if (this.GetStringValue() != "W")
                                {
                                    break;
                                }
                                info.GpsLongitudeRef = ExifGpsLongitudeRef.West;
                                return;
                            }
                        case ExifGps.Longitude:
                            {
                                if (this.Components != 3)
                                {
                                    break;
                                }
                                info.GpsLongitude[0] = this.GetNumericValue(0);
                                info.GpsLongitude[1] = this.GetNumericValue(1);
                                info.GpsLongitude[2] = this.GetNumericValue(2);
                                break;
                            }
                        default:
                            {
                                return;
                            }
                    }
                }
            }
            else
            {
                tag = (ExifId)this.Tag;
                if (tag > ExifId.DateTime)
                {
                    if (tag > ExifId.ExposureTime)
                    {
                        if (tag == ExifId.FNumber)
                        {
                            info.FNumber = this.GetNumericValue(0);
                            return;
                        }
                        if (tag == ExifId.FlashUsed)
                        {
                            info.Flash = (ExifFlash)this.GetInt(0);
                            return;
                        }
                        if (tag != ExifId.UserComment)
                        {
                            return;
                        }
                        info.UserComment = this.GetStringValue();
                        return;
                    }
                    if (tag == ExifId.Artist)
                    {
                        info.Artist = this.GetStringValue();
                        return;
                    }
                    switch (tag)
                    {
                        case ExifId.ThumbnailOffset:
                            {
                                info.ThumbnailOffset = this.GetInt(0);
                                return;
                            }
                        case ExifId.ThumbnailLength:
                            {
                                info.ThumbnailSize = this.GetInt(0);
                                return;
                            }
                        default:
                            {
                                switch (tag)
                                {
                                    case ExifId.Copyright:
                                        {
                                            info.Copyright = this.GetStringValue();
                                            return;
                                        }
                                    case ExifId.ThumbnailOffset | ExifId.Copyright:
                                        {
                                            break;
                                        }
                                    case ExifId.ExposureTime:
                                        {
                                            info.ExposureTime = this.GetNumericValue(0);
                                            return;
                                        }
                                    default:
                                        {
                                            return;
                                        }
                                }
                                break;
                            }
                    }
                }
                else
                {
                    if (tag > ExifId.Orientation)
                    {
                        switch (tag)
                        {
                            case ExifId.XResolution:
                                {
                                    info.XResolution = this.GetNumericValue(0);
                                    return;
                                }
                            case ExifId.YResolution:
                                {
                                    info.YResolution = this.GetNumericValue(0);
                                    return;
                                }
                            default:
                                {
                                    if (tag == ExifId.ResolutionUnit)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        goto Label0;
                                    }
                                }
                        }
                        info.ResolutionUnit = (ExifUnit)this.GetInt(0);
                        return;
                    }
                    switch (tag)
                    {
                        case ExifId.ImageWidth:
                            {
                                info.Width = this.GetInt(0);
                                return;
                            }
                        case ExifId.ImageHeight:
                            {
                                info.Height = this.GetInt(0);
                                return;
                            }
                        default:
                            {
                                switch (tag)
                                {
                                    case ExifId.Description:
                                        {
                                            info.Description = this.GetStringValue();
                                            return;
                                        }
                                    case ExifId.Make:
                                        {
                                            info.Make = this.GetStringValue();
                                            return;
                                        }
                                    case ExifId.Model:
                                        {
                                            info.Model = this.GetStringValue();
                                            return;
                                        }
                                    case ExifId.ImageWidth | ExifId.ImageHeight | ExifId.Model:
                                        {
                                            break;
                                        }
                                    case ExifId.Orientation:
                                        {
                                            info.Orientation = (ExifOrientation)this.GetInt(0);
                                            return;
                                        }
                                    default:
                                        {
                                            return;
                                        }
                                }
                                break;
                            }
                    }
                }
            }
            return;
        Label0:
            switch (tag)
            {
                case ExifId.Software:
                    {
                        info.Software = this.GetStringValue();
                        return;
                    }
                case ExifId.DateTime:
                    {
                        info.DateTime = this.GetStringValue();
                        return;
                    }
                default:
                    {
                        return;
                    }
            }
        }

        private double ReadDouble(int offset)
        {
            return ExifIO.ReadDouble(this.Data, offset, this.LittleEndian);
        }

        private int ReadInt(int offset)
        {
            return ExifIO.ReadInt(this.Data, offset, this.LittleEndian);
        }

        private short ReadShort(int offset)
        {
            return ExifIO.ReadShort(this.Data, offset, this.LittleEndian);
        }

        private float ReadSingle(int offset)
        {
            return ExifIO.ReadSingle(this.Data, offset, this.LittleEndian);
        }

        private uint ReadUInt(int offset)
        {
            return ExifIO.ReadUInt(this.Data, offset, this.LittleEndian);
        }

        private ushort ReadUShort(int offset)
        {
            return ExifIO.ReadUShort(this.Data, offset, this.LittleEndian);
        }

        //public override string ToString()
        //{
        //    StringBuilder stringBuilder = new StringBuilder(64);
        //    stringBuilder.Append("0x");
        //    int tag = this.Tag;
        //    stringBuilder.Append(tag.ToString("X4"));
        //    stringBuilder.Append("-");
        //    stringBuilder.Append((ExifId)this.Tag.ToString());
        //    if (this.Components > 0)
        //    {
        //        stringBuilder.Append(": (");
        //        stringBuilder.Append(this.GetStringValue(0));
        //        if (this.Format != ExifTagFormat.UNDEFINED && this.Format != ExifTagFormat.STRING)
        //        {
        //            for (int i = 1; i < this.Components; i++)
        //            {
        //                stringBuilder.Append(string.Concat(", ", this.GetStringValue(i)));
        //            }
        //        }
        //        stringBuilder.Append(")");
        //    }
        //    return stringBuilder.ToString();
        //}
    }
}