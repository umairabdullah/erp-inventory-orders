using Erp.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Erp.Domain.ValueObjects
{
    public sealed record class Sku
    {
        private static readonly Regex Pattern = new("^[A-Z0-9-]{3,30}$", RegexOptions.Compiled);
        public string Value { get; }

        private Sku(string value)
        {
            Value = value;
        }

        public static Sku Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new DomainException("SKU is required");
            }

            var normalized = value.Trim().ToUpperInvariant();

            if (!Pattern.IsMatch(normalized)) 
            {
                throw new DomainException("SKU must be 3 to 30 characters: letters, digits, or hyphens.");
            }
            return new Sku(normalized);
        }

        public override string ToString() => Value;
       
    }
}