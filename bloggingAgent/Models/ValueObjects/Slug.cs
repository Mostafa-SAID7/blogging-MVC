using System;
using System.Text.RegularExpressions;

namespace BloggingAgent.Models.ValueObjects
{
    public class Slug : IEquatable<Slug>
    {
        private readonly string _value;
        private static readonly Regex SlugRegex = new Regex(@"^[a-z0-9]+(?:-[a-z0-9]+)*$",
            RegexOptions.Compiled);

        public Slug(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Slug cannot be null or empty", nameof(value));

            if (!IsValidSlug(value))
                throw new ArgumentException("Invalid slug format. Only lowercase letters, numbers, and hyphens are allowed.", nameof(value));

            _value = value.ToLowerInvariant().Trim();
        }

        public string Value => _value;

        public static Slug GenerateFromTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be null or empty", nameof(title));

            var slug = title.ToLowerInvariant()
                           .Replace(" ", "-")
                           .Replace(".", "")
                           .Replace(",", "")
                           .Replace("?", "")
                           .Replace("!", "")
                           .Replace(":", "")
                           .Replace(";", "")
                           .Replace("\"", "")
                           .Replace("'", "")
                           .Replace("&", "and")
                           .Replace("+", "plus");

            // Remove any remaining invalid characters
            slug = Regex.Replace(slug, @"[^a-z0-9\-]", "");

            // Remove multiple consecutive hyphens
            slug = Regex.Replace(slug, @"-+", "-");

            // Remove leading/trailing hyphens
            slug = slug.Trim('-');

            // Ensure minimum length
            if (slug.Length < 3)
                slug += "-post";

            // Ensure maximum length
            if (slug.Length > 100)
                slug = slug.Substring(0, 100).TrimEnd('-');

            return new Slug(slug);
        }

        public bool IsValid => IsValidSlug(_value);

        private static bool IsValidSlug(string slug)
        {
            return !string.IsNullOrWhiteSpace(slug) &&
                   slug.Length >= 3 &&
                   slug.Length <= 100 &&
                   SlugRegex.IsMatch(slug);
        }

        public override string ToString() => _value;

        public override bool Equals(object obj) => Equals(obj as Slug);

        public bool Equals(Slug other)
        {
            if (other is null) return false;
            return string.Equals(_value, other._value, StringComparison.Ordinal);
        }

        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(_value);

        public static bool operator ==(Slug left, Slug right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(Slug left, Slug right) => !(left == right);

        public static implicit operator string(Slug slug) => slug?._value;

        public static explicit operator Slug(string slug) => new Slug(slug);
    }
}