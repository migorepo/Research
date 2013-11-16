using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Wp8.Framework.Utils.Common
{
    public static class Extensions
    {
        /// <summary>
        /// This method can convert any IEnumerable list to ObservableCollection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static ObservableCollection<T> ToCollection<T>(this IEnumerable<T> list)
        {
            ObservableCollection<T> returnValue = null;
            if (list != null)
            {
                returnValue = new ObservableCollection<T>();
                foreach (T item in list)
                    returnValue.Add(item);
            }
            return returnValue;
        }

        /// <summary>
        /// This method can convert any IEnumerable list to List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<T> ToList<T>(this ObservableCollection<T> list)
        {
            IList<T> returnValue = null;
            if (list != null)
            {
                returnValue = Enumerable.ToList(list);
            }
            return returnValue;
        }

        /// <summary>
        /// Extension to check the email address entered is valid or not
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsValidEmailAddress(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return false;
            var regex = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            return regex.IsMatch(s) && !s.EndsWith(".");
        }

        /// <summary>
        /// Method for parsing query string parameters 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static IDictionary<string, string> ParseQueryString(this string query)
        {
            var result = new Dictionary<string, string>();
            if (string.IsNullOrWhiteSpace(query))
            {
                return result;
            }

            if (query.Contains("?"))
            {
                int idx = query.IndexOf("?", StringComparison.Ordinal) + 1;
                query = query.Substring(idx, query.Length - idx);
            }

            string decoded = HttpUtility.HtmlDecode(query);
            int decodedLength = decoded.Length;
            int namePos = 0;
            bool first = true;

            while (namePos <= decodedLength)
            {
                int valuePos = -1, valueEnd = -1;
                for (int q = namePos; q < decodedLength; q++)
                {
                    if (valuePos == -1 && decoded[q] == '=')
                    {
                        valuePos = q + 1;
                    }
                    else if (decoded[q] == '&')
                    {
                        valueEnd = q;
                        break;
                    }
                }

                if (first)
                {
                    first = false;
                    if (decoded[namePos] == '?')
                    {
                        namePos++;
                    }
                }

                string name;
                if (valuePos == -1)
                {
                    name = null;
                    valuePos = namePos;
                }
                else
                {
                    name = HttpUtility.UrlDecode(decoded.Substring(namePos, valuePos - namePos - 1));
                }

                if (valueEnd < 0)
                {
                    namePos = -1;
                    valueEnd = decoded.Length;
                }
                else
                {
                    namePos = valueEnd + 1;
                }

                string value = HttpUtility.UrlDecode(decoded.Substring(valuePos, valueEnd - valuePos));

                if (!string.IsNullOrEmpty(name))
                {
                    result[name] = value;
                }

                if (namePos == -1)
                {
                    break;
                }
            }
            return result;
        }


        /// <summary>
        /// Method to build comma separated string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static string ToCommaDelimitedString<T>(this IEnumerable<T> items)
        {
            var sb = new StringBuilder();
            foreach (var item in items)
            {
                sb.Append(item);
                sb.Append(',');
            }
            if (sb.Length >= 1) sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        /// <summary>
        /// Method to build a string from ',' separated list
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static List<string> ToListofStrings(this string value)
        {
            var collection = value.Split(',');
            if (collection.Any())
                return collection.ToList();
            return null;
        }

        /// <summary>
        /// Extension method to convert a stream input to byte array
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] ReadAsBytes(this Stream input)
        {
            var array = new byte[16384];
            byte[] result;
            using (var memoryStream = new MemoryStream())
            {
                int num;
                while ((num = input.Read(array, 0, array.Length)) > 0)
                {
                    memoryStream.Write(array, 0, num);
                }
                result = memoryStream.ToArray();
            }
            return result;
        }

        /// <summary>
        /// Extension method that converts ordinary points to Stylus Point
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static StylusPoint ConvertToStylusPoint(this Point position)
        {
            return new StylusPoint(position.X, position.Y);
        }

        /// <summary>
        /// Method that converts color code to know colors
        /// </summary>
        /// <param name="colorName"></param>
        /// <returns></returns>
        public static Color ToKnownColor(this string colorName)
        {
            switch (colorName)
            {
                case "Black":
                    return Colors.Black;
                case "White":
                    return Colors.White;
                case "Gray":
                    return Colors.Gray;
                case "Pink":
                    return Color.FromArgb(225, 225, 192, 203);
                case "Yellow":
                    return Colors.Yellow;
                case "Red":
                    return Colors.Red;
                case "Green":
                    return Colors.Green;
                case "Blue":
                    return Colors.Blue;
                case "Purple":
                    return Colors.Purple;
                case "Orange":
                    return Colors.Orange;
                default:
                    return Colors.Black;

            }
        }

    }
}
