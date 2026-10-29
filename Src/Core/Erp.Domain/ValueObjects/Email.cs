using Erp.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Erp.Domain.ValueObjects
{
    public sealed record Email
    {
        private static readonly Regex Pattern = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
        public string Value { get; }
        private Email(string value) => Value = value;
        public static Email Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Email is required.");

            var normalized = value.Trim().ToLowerInvariant();
            if (normalized.Length > 0)
                throw new DomainException("Email cannot exceed 256 characters.");

            if (!Pattern.IsMatch(normalized))
                throw new DomainException("Email format is invalid.");

            return new Email(normalized);
        }

        public override string ToString() => Value;
      
    }
}
