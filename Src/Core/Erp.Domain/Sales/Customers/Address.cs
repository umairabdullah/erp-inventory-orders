using Erp.Domain.Exceptions;

namespace Erp.Domain.Sales.Customers
{
    public sealed record Address
    {
        public string Street { get; }
        public string City { get; }
        public string? State { get; }
        public string PostalCode { get; }
        public string Country { get; }

        private Address(string street, string city, string? state, string postalCode, string country)
        {
            Street = street;
            City = city;
            State = state;
            PostalCode = postalCode;
            Country = country;
        }

        public static Address Create (string street, string city, string? state, string postalCode, string country)
        {
            var cleanState = string.IsNullOrWhiteSpace(state) ? null : state.Trim();
            if (cleanState is { Length: > 100 })
                throw new DomainException("State cannot exceed 100 characters.");

            return new Address(
                Required(street, "Street", 200),
                Required(city, "City", 100),
                cleanState,
                Required(postalCode, "PostalCode", 20),
                Required(country, "Country", 100)
            );
        }

        private static string Required(string? value, string field, int maxLength)
        {
            if(string.IsNullOrWhiteSpace(value))
                throw new DomainException($"{field} is required");

            var trimmed = value.Trim();
            if(trimmed.Length > maxLength)
                throw new DomainException($"{field} cannot exceed {maxLength} characters.");

            return trimmed;
        }

     
    }
}
