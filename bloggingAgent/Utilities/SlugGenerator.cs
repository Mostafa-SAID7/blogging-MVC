using System;
using System.Text.RegularExpressions;
using BloggingAgent.Extensions;

namespace BloggingAgent.Utilities
{
    public static class SlugGenerator
    {
        public static string GenerateSlug(string text, int maxLength = 100)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            // Use the extension method for basic slug generation
            var slug = text.ToSlug();

            // Ensure it's not too long
            if (slug.Length > maxLength)
            {
                slug = slug.Substring(0, maxLength);
                // Make sure we don't cut in the middle of a word
                var lastHyphen = slug.LastIndexOf('-');
                if (lastHyphen > maxLength / 2)
                {
                    slug = slug.Substring(0, lastHyphen);
                }
            }

            // Ensure uniqueness by adding timestamp if needed
            // This would typically be handled at the repository level

            return slug;
        }

        public static string GenerateUniqueSlug(string baseSlug, Func<string, bool> existsCheck)
        {
            var slug = baseSlug;
            var counter = 1;

            while (existsCheck(slug))
            {
                slug = $"{baseSlug}-{counter}";
                counter++;
            }

            return slug;
        }

        public static bool IsValidSlug(string slug)
        {
            if (string.IsNullOrEmpty(slug))
                return false;

            // Slug should only contain lowercase letters, numbers, and hyphens
            return Regex.IsMatch(slug, @"^[a-z0-9]+(?:-[a-z0-9]+)*$");
        }

        public static string SanitizeForSlug(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            // Remove HTML tags
            text = Regex.Replace(text, @"<[^>]+>", "");

            // Remove markdown formatting
            text = Regex.Replace(text, @"[#*`~\[\]\(\)]", "");

            // Remove extra whitespace
            text = Regex.Replace(text, @"\s+", " ").Trim();

            return text;
        }
    }
}