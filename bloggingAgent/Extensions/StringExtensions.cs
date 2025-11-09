using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace BloggingAgent.Extensions
{
    public static class StringExtensions
    {
        public static string ToSlug(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            // Convert to lowercase
            text = text.ToLowerInvariant();

            // Remove diacritics
            text = RemoveDiacritics(text);

            // Replace spaces and special characters with hyphens
            text = Regex.Replace(text, @"[^a-z0-9\s-]", "");

            // Replace multiple spaces or hyphens with single hyphen
            text = Regex.Replace(text, @"[\s-]+", "-");

            // Trim hyphens from start and end
            text = text.Trim('-');

            return text;
        }

        public static string Truncate(this string text, int maxLength, string suffix = "...")
        {
            if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
                return text;

            var truncated = text.Substring(0, maxLength - suffix.Length);
            var lastSpace = truncated.LastIndexOf(' ');

            if (lastSpace > 0)
                truncated = truncated.Substring(0, lastSpace);

            return truncated + suffix;
        }

        public static string ExtractExcerpt(this string content, int maxLength = 150)
        {
            if (string.IsNullOrEmpty(content))
                return string.Empty;

            // Remove HTML tags
            var plainText = Regex.Replace(content, @"<[^>]+>", "");

            // Remove markdown formatting
            plainText = Regex.Replace(plainText, @"[#*`~\[\]\(\)]", "");

            return plainText.Truncate(maxLength);
        }

        public static int WordCount(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            var words = text.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            return words.Length;
        }

        public static string CapitalizeFirst(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return char.ToUpper(text[0]) + text.Substring(1).ToLower();
        }

        public static string ToTitleCase(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text.ToLower());
        }

        private static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(System.Text.NormalizationForm.FormD);
            var stringBuilder = new System.Text.StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(System.Text.NormalizationForm.FormC);
        }
    }
}