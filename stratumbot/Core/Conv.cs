using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace stratumbot.Core
{
    public static class Conv
    {
        /// <summary>
        /// Преобразование decimal в строку. Если tickSize больше по длинне, то в конце будут добавлены нули.
        /// Осторожно! String.Format округляет в ближайшую сторону! Подавать уже правильно округлённые значения!
        /// </summary>
        public static string s8(decimal? value, decimal tickSize = 8)
        {
            if (value == null)
                return null;

            if (tickSize == (decimal)0.00000001 || tickSize == 8)
            {
                return String.Format("{0:F8}", value);
            }

            if (tickSize == (decimal)0.00000010 || tickSize == 7)
            {
                return String.Format("{0:F7}", value);
            }

            if (tickSize == (decimal)0.00000100 || tickSize == 6)
            {
                return String.Format("{0:F6}", value);
            }

            if (tickSize == (decimal)0.00001000 || tickSize == 5)
            {
                return String.Format("{0:F5}", value);
            }

            if (tickSize == (decimal)0.00010000 || tickSize == 4)
            {
                return String.Format("{0:F4}", value);
            }

            if (tickSize == (decimal)0.00100000 || tickSize == 3)
            {
                return String.Format("{0:F3}", value);
            }

            if (tickSize == (decimal)0.01000000 || tickSize == 2)
            {
                return String.Format("{0:F2}", value);
            }

            if (tickSize == (decimal)0.10000000 || tickSize == 1)
            {
                return String.Format("{0:F1}", value);
            }

            if (tickSize == (decimal)1.00000000 || tickSize == 0)
            {
                return String.Format("{0:F0}", value);
            }

            return String.Format("{0:F8}", value);
        }

        /// <summary>
        /// Parse string to decomal (any separator)
        /// </summary>
        public static decimal dec(string value)
        {
            string sep1 = ",";
            string sep2 = ".";

            // Русский разделитель
            if (Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator.Equals(","))
            {
                sep1 = ".";
                sep2 = ",";
            }
            // Английский разделитель
            if (Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator.Equals("."))
            {
                sep1 = ",";
                sep2 = ".";
            }

            value = value.Replace(" ", "");

            return decimal.Parse(value.Replace(sep1, sep2));
        }

        /// <summary>
        /// Convert JToken to string and Parse it to decomal (any separator)
        /// </summary>
        public static decimal dec(JToken value)
        {
            return dec(value.ToString());
        }

        /// <summary>
        /// Clean: space, %, шт., шт, pieces, pieces.
        /// </summary>
        public static decimal dec(string value, bool clean)
        {
            value = value.Replace("%", "");
            value = value.Replace("шт.", "");
            value = value.Replace("шт", "");
            value = value.Replace("pieces.", "");
            value = value.Replace("pieces", "");
            return dec(value);
        }

        /// <summary>
        /// Clean: space, %, шт., шт, pieces, pieces.
        /// </summary>
        public static int cleanInt(string value)
        {
            value = value.Replace("%", "");
            value = value.Replace("#", "");
            value = value.Replace("шт.", "");
            value = value.Replace("шт", "");
            value = value.Replace("pieces.", "");
            value = value.Replace("pieces", "");
            value = value.Replace(" ", "");
            return int.Parse(value);
        }

        /// <summary>
        /// Удаляет все символы кроме цифр
        /// </summary>
        public static decimal GetClearInt(string value)
        {
            int result;
            int.TryParse(string.Join("", value.Where(c => char.IsDigit(c))), out result);
            return result;
        }

    }
}
