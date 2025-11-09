using System;
using System.Globalization;

namespace BloggingAgent.Models.ValueObjects
{
    public class Money : IEquatable<Money>
    {
        public decimal Amount { get; private set; }
        public string Currency { get; private set; }

        private static readonly HashSet<string> ValidCurrencies = new HashSet<string>
        {
            "USD", "EUR", "GBP", "JPY", "CAD", "AUD", "CHF", "CNY", "INR"
        };

        public Money(decimal amount, string currency = "USD")
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative", nameof(amount));

            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency cannot be null or empty", nameof(currency));

            if (!ValidCurrencies.Contains(currency.ToUpperInvariant()))
                throw new ArgumentException($"Invalid currency: {currency}", nameof(currency));

            Amount = Math.Round(amount, 2);
            Currency = currency.ToUpperInvariant();
        }

        public Money Add(Money other)
        {
            if (Currency != other.Currency)
                throw new InvalidOperationException("Cannot add money with different currencies");

            return new Money(Amount + other.Amount, Currency);
        }

        public Money Subtract(Money other)
        {
            if (Currency != other.Currency)
                throw new InvalidOperationException("Cannot subtract money with different currencies");

            if (other.Amount > Amount)
                throw new InvalidOperationException("Cannot subtract more than available amount");

            return new Money(Amount - other.Amount, Currency);
        }

        public Money Multiply(decimal factor)
        {
            if (factor < 0)
                throw new ArgumentException("Factor cannot be negative", nameof(factor));

            return new Money(Amount * factor, Currency);
        }

        public Money Divide(decimal divisor)
        {
            if (divisor <= 0)
                throw new ArgumentException("Divisor must be positive", nameof(divisor));

            return new Money(Amount / divisor, Currency);
        }

        public string Format() => $"{Amount:N2} {Currency}";

        public string Format(CultureInfo culture)
        {
            var currencySymbol = GetCurrencySymbol(Currency);
            return $"{currencySymbol}{Amount:N2}";
        }

        private static string GetCurrencySymbol(string currency)
        {
            return currency.ToUpperInvariant() switch
            {
                "USD" => "$",
                "EUR" => "€",
                "GBP" => "£",
                "JPY" => "¥",
                "CAD" => "C$",
                "AUD" => "A$",
                "CHF" => "CHF",
                "CNY" => "¥",
                "INR" => "₹",
                _ => currency
            };
        }

        public override string ToString() => Format();

        public override bool Equals(object obj) => Equals(obj as Money);

        public bool Equals(Money other)
        {
            if (other is null) return false;
            return Amount == other.Amount && Currency == other.Currency;
        }

        public override int GetHashCode() => HashCode.Combine(Amount, Currency);

        public static bool operator ==(Money left, Money right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(Money left, Money right) => !(left == right);

        public static Money operator +(Money left, Money right) => left.Add(right);

        public static Money operator -(Money left, Money right) => left.Subtract(right);

        public static Money operator *(Money money, decimal factor) => money.Multiply(factor);

        public static Money operator /(Money money, decimal divisor) => money.Divide(divisor);
    }
}