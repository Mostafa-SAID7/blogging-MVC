using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BloggingAgent.Models.ValueObjects
{
    public class EmailAddress : IEquatable<EmailAddress>
    {
        private readonly string _value;

        private static readonly Regex EmailRegex = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public EmailAddress(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email address cannot be null or empty", nameof(email));

            if (!IsValidEmail(email))
                throw new ArgumentException("Invalid email address format", nameof(email));

            _value = email.ToLowerInvariant().Trim();
        }

        public string Value => _value;

        public string Domain => _value.Split('@')[1];

        public string LocalPart => _value.Split('@')[0];

        public bool IsValid => IsValidEmail(_value);

        private static bool IsValidEmail(string email)
        {
            return !string.IsNullOrWhiteSpace(email) && EmailRegex.IsMatch(email);
        }

        public override string ToString() => _value;

        public override bool Equals(object obj) => Equals(obj as EmailAddress);

        public bool Equals(EmailAddress other)
        {
            if (other is null) return false;
            return string.Equals(_value, other._value, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(_value);

        public static bool operator ==(EmailAddress left, EmailAddress right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(EmailAddress left, EmailAddress right) => !(left == right);

        public static implicit operator string(EmailAddress email) => email?._value;

        public static explicit operator EmailAddress(string email) => new EmailAddress(email);
    }
}