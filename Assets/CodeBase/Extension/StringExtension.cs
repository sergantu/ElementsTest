using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace CodeBase.Extension
{
    [PublicAPI]
    public static class StringExtension
    {
        public static string Truncate(this string value, int maxLength, string? replace = null)
        {
            if (value.Length <= maxLength) {
                return value;
            }
            if (string.IsNullOrEmpty(replace)) {
                return value[..maxLength];
            }
            return value[..(maxLength - replace!.Length)] + replace;
        }

        public static string StripBOM(this string value)
        {
            value = value.Trim();
            string encodedValue;
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF) {
                encodedValue = Encoding.UTF8.GetString(bytes, 3, value.Length - 3);
            } else {
                encodedValue = Encoding.UTF8.GetString(bytes);
            }
            return encodedValue;
        }

        public static string Capitalize(this string self)
        {
            if (self.Length == 0) {
                return self;
            }
            return char.ToUpperInvariant(self[0]) + (self.Length > 1 ? self[1..] : "");
        }

        public static string Uncapitalize(this string self)
        {
            if (self.Length == 0) {
                return self;
            }
            return char.ToLowerInvariant(self[0]) + (self.Length > 1 ? self[1..] : "");
        }

        public static string CamelCaseToUnderscore(this string self)
        {
            return string.Concat(self.Select((x, i) => i > 0 && char.IsUpper(x)
                                                               ? "_" + char.ToUpperInvariant(x)
                                                               : char.ToUpperInvariant(x).ToString())
                                     .ToArray());
        }

        public static string UnderscoreToCamelCase(this string self)
        {
            var builder = new StringBuilder();
            string str = self.ToLowerInvariant();
            for (int i = 0; i < str.Length; i++) {
                char c = str[i];
                if (i == 0 && c != '_') {
                    builder.Append(char.ToUpperInvariant(c));
                } else if (c == '_' && i != str.Length - 1) {
                    builder.Append(char.ToUpperInvariant(str[i + 1]));
                    i++;
                } else {
                    builder.Append(c);
                }
            }
            return builder.ToString();
        }
    }
}