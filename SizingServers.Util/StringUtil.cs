/*
Original author: Dieter Vandroemme, dev at Sizing Servers Lab (https://www.sizingservers.be) @ University College of West-Flanders, Department GKG
Written in 2008

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Text;
using System.Threading;

namespace SizingServers.Util {
    /// <summary>String helper class. With this you can generate names and format numeric values.</summary>
    public static class StringUtil {
        /// <summary>
        /// No Scientific notation ToString().
        /// </summary>
        /// <param name="f"></param>
        /// <param name="thousandSeparator"></param>
        /// <returns></returns>
        public static string FloatToLongString(float f, bool thousandSeparator = false) {
            if (Single.IsNaN(f) || Single.IsPositiveInfinity(f) || Single.IsNegativeInfinity(f))
                return f.ToString();

            return NumberToLongString(f, thousandSeparator && f > 999);
        }

        /// <summary>
        /// No Scientific notation ToString().
        /// </summary>
        /// <param name="d"></param>
        /// <param name="thousandSeparator"></param>
        /// <returns></returns>
        public static string DoubleToLongString(double d, bool thousandSeparator = false) {
            if (Double.IsNaN(d) || Double.IsPositiveInfinity(d) || Double.IsNegativeInfinity(d))
                return d.ToString();

            return NumberToLongString(d, thousandSeparator && d > 999);
        }

        /// <summary>
        /// No Scientific notation ToString().
        /// </summary>
        /// <param name="o"></param>
        /// <param name="thousandSeparator"></param>
        /// <returns></returns>
        private static string NumberToLongString(object o, bool thousandSeparator) {
            string s = o.ToString().ToUpper();

            //if string representation was collapsed from scientific notation, just return it
            if (!s.Contains("E")) return s;

            char separator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];

            string[] exponentParts = s.Split('E');
            string[] decimalParts = exponentParts[0].Split(separator);

            //fix missing decimal point
            if (decimalParts.Length == 1) decimalParts = new[] { exponentParts[0], "0" };
            string newNumber = decimalParts[0] + decimalParts[1];

            int exponentValue = int.Parse(exponentParts[1]);
            //positive exponent
            if (exponentValue > 0)
                s = newNumber + GetZeros(exponentValue - decimalParts[1].Length);
            else
                //negative exponent
                s = ("0" + separator + GetZeros(exponentValue + decimalParts[0].Length) + newNumber).TrimEnd('0');

            if (thousandSeparator)
                s = SeparateThousands(s);

            return s;
        }

        private static string GetZeros(int zeroCount) {
            if (zeroCount < 0)
                zeroCount = Math.Abs(zeroCount);

            return new string('0', zeroCount);
        }

        private static string SeparateThousands(string s) {
            string separator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            string[] split = s.Split(separator[0]);

            var sb = new StringBuilder(split[0].Length + 1);

            int j = 0;
            for (int i = split[0].Length - 1; i != -1; i--) {
                sb.Append(split[0][i]);
                if (++j == 3) {
                    j = 0;
                    sb.Append(" ");
                }
            }

            s = ReverseString(sb.ToString().TrimEnd());
            if (split.Length == 2 && !string.IsNullOrEmpty(split[1]))
                s += separator + split[1];

            return s;
        }

        /// <summary>
        /// Reverse a string.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ReverseString(string s) {
            var sb = new StringBuilder(s.Length); ;
            for (int i = s.Length - 1; i != -1; i--)
                sb.Append(s[i]);

            return sb.ToString();
        }
    }
}