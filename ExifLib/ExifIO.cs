using System;

namespace ExifLib
{
    public static class ExifIO
    {
        public static double ReadDouble(byte[] Data, int offset, bool littleEndian)
        {
            if (littleEndian && BitConverter.IsLittleEndian || !littleEndian && !BitConverter.IsLittleEndian)
            {
                return BitConverter.ToDouble(Data, offset);
            }
            byte[] data = new byte[] { Data[offset + 7], Data[offset + 6], Data[offset + 5], Data[offset + 4], Data[offset + 3], Data[offset + 2], Data[offset + 1], Data[offset] };
            byte[] numArray = data;
            return BitConverter.ToDouble(numArray, 0);
        }

        public static int ReadInt(byte[] Data, int offset, bool littleEndian)
        {
            if (littleEndian && BitConverter.IsLittleEndian || !littleEndian && !BitConverter.IsLittleEndian)
            {
                return BitConverter.ToInt32(Data, offset);
            }
            byte[] data = new byte[] { Data[offset + 3], Data[offset + 2], Data[offset + 1], Data[offset] };
            byte[] numArray = data;
            return BitConverter.ToInt32(numArray, 0);
        }

        public static short ReadShort(byte[] Data, int offset, bool littleEndian)
        {
            if (littleEndian && BitConverter.IsLittleEndian || !littleEndian && !BitConverter.IsLittleEndian)
            {
                return BitConverter.ToInt16(Data, offset);
            }
            byte[] data = new byte[] { Data[offset + 1], Data[offset] };
            byte[] numArray = data;
            return BitConverter.ToInt16(numArray, 0);
        }

        public static float ReadSingle(byte[] Data, int offset, bool littleEndian)
        {
            if (littleEndian && BitConverter.IsLittleEndian || !littleEndian && !BitConverter.IsLittleEndian)
            {
                return BitConverter.ToSingle(Data, offset);
            }
            byte[] data = new byte[] { Data[offset + 3], Data[offset + 2], Data[offset + 1], Data[offset] };
            byte[] numArray = data;
            return BitConverter.ToSingle(numArray, 0);
        }

        public static uint ReadUInt(byte[] Data, int offset, bool littleEndian)
        {
            if (littleEndian && BitConverter.IsLittleEndian || !littleEndian && !BitConverter.IsLittleEndian)
            {
                return BitConverter.ToUInt32(Data, offset);
            }
            byte[] data = new byte[] { Data[offset + 3], Data[offset + 2], Data[offset + 1], Data[offset] };
            byte[] numArray = data;
            return BitConverter.ToUInt32(numArray, 0);
        }

        public static ushort ReadUShort(byte[] Data, int offset, bool littleEndian)
        {
            if (littleEndian && BitConverter.IsLittleEndian || !littleEndian && !BitConverter.IsLittleEndian)
            {
                return BitConverter.ToUInt16(Data, offset);
            }
            byte[] data = new byte[] { Data[offset + 1], Data[offset] };
            byte[] numArray = data;
            return BitConverter.ToUInt16(numArray, 0);
        }
    }
}
