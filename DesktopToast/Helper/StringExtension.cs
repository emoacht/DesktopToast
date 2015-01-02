using System;
using System.Text;

namespace DesktopToast.Helper
{
    internal static class StringExtension
    {
        /// <summary>
        /// Convert camel-cased string to camel-cased string with separator.
        /// </summary>
        /// <param name="source">Source String</param>
        /// <param name="separator">Separator Char</param>
        /// <returns>String with separator</returns>
        public static string ToCamelWithSeparator(this string source, char separator)
        {
            if (String.IsNullOrEmpty(source))
                return source;

            var sourceArray = source.ToCharArray();

            var sb = new StringBuilder(sourceArray[0].ToString());

            for (int i = 1; i <= sourceArray.Length - 1; i++) // Index 0 is skipped.
            {
                if (Char.IsUpper(sourceArray[i]) && !Char.IsUpper(sourceArray[i - 1]))
                    sb.Append(separator);

                sb.Append(sourceArray[i]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Determine if specified strings are equal or both strings are null or empty.
        /// </summary>
        /// <param name="a">String to compare</param>
        /// <param name="b">String to compare</param>
        /// <param name="comparisonType">StringComparison</param>
        /// <returns>True if the strings are equal or both strings are null or empty</returns>
        public static bool EqualsOrIsNullOrEmpty(this string a, string b, StringComparison comparisonType)
        {
            if (String.IsNullOrEmpty(a))
                return String.IsNullOrEmpty(b);

            return String.Equals(a, b, comparisonType);
        }
    }
}