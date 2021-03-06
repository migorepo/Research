﻿using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using Color = Windows.UI.Color;

namespace Wp8.Framework.Utils.Utils
{
    public static class ColorUtility
    {
        static readonly Random Rnd = new Random();

        public static Color Mix(Color foreground, double alpha, Color background)
        {
            var diff = 1.0 - alpha;
            var color = Color.FromArgb(foreground.A,
                (byte)(foreground.R * alpha + background.R * diff),
                (byte)(foreground.G * alpha + background.G * diff),
                (byte)(foreground.B * alpha + background.B * diff));
            return color;
        }

        public static Color RemoveAlpha(Color foreground, Color background)
        {
            if (foreground.A == 255)
                return foreground;

            var alpha = foreground.A / 255.0;
            var diff = 1.0 - alpha;
            return Color.FromArgb(255,
                (byte)(foreground.R * alpha + background.R * diff),
                (byte)(foreground.G * alpha + background.G * diff),
                (byte)(foreground.B * alpha + background.B * diff));
        }

        public static Color ChangeAlpha(Color color, byte alpha)
        {
            return Color.FromArgb(alpha, color.R, color.G, color.B);
        }

        public static Color ChangeAlpha(Color color, string alpha)
        {
            var value = UInt32.Parse(alpha, NumberStyles.HexNumber);
            return ChangeAlpha(color, (byte)(value & 0xff));
        }

        public static string ToHex(Color color, bool includeAlpha = false)
        {
            if (includeAlpha)
                return "#" +
                    Convert.ToInt32(color.A).ToString("X2") +
                    Convert.ToInt32(color.R).ToString("X2") +
                    Convert.ToInt32(color.G).ToString("X2") +
                    Convert.ToInt32(color.B).ToString("X2");
            return "#" +
                Convert.ToInt32(color.R).ToString("X2") +
                Convert.ToInt32(color.G).ToString("X2") +
                Convert.ToInt32(color.B).ToString("X2");
        }

        public static Color FromHex(string colorCode)
        {
            colorCode = colorCode.Replace("#", "");
            if (colorCode.Length == 6)
                colorCode = "FF" + colorCode;
            return FromHex(UInt32.Parse(colorCode, NumberStyles.HexNumber));
        }

        public static Color FromHex(uint argb)
        {
            return Color.FromArgb((byte)((argb & -16777216) >> 0x18),
                              (byte)((argb & 0xff0000) >> 0x10),
                              (byte)((argb & 0xff00) >> 8),
                              (byte)(argb & 0xff));
        }

        public static Color GetRandomColour()
        {
            var colorProperties = typeof(Colors).GetProperties(BindingFlags.Static | BindingFlags.Public);
            var colors = colorProperties.Select(prop => (Color)prop.GetValue(null, null));

            var enumerable = colors as Color[] ?? colors.ToArray();
            var index = Rnd.Next(enumerable.Count());

            return enumerable.ElementAt(index);
        }
    }
}
