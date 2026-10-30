using Erp.Domain.Common;
using Erp.Domain.Exceptions;
using Erp.Domain.ValueObjects;

namespace Erp.Domain.Sales.Customers
{
    public sealed class Customer : AggregateRoot
    {
        public string Name { get; private set; } = string.Empty;
        public Email Email { get; private set; } = null!;
        public string? Phone { get; private set; }
        public Address Address { get; private set; } = null!;
        public bool IsActive { get; private set; } = true;

        private Customer() { }
        public static Customer Create(string name, Email email, string? phone, Address address)
        {
            var customer = new Customer();
            customer.Apply(name, email, phone, address);
            return customer;
        }
        public void Update(string name, Email email, string? phone, Address address)
        {
            Apply(name, email, phone, address);
            MarkUpdated();
        }

        public void Activate()
        {
            IsActive = true;
            MarkUpdated();
        }

        public void Deactivate()
        {
            IsActive = false;
            MarkUpdated();
        }

        private void Apply(string name, Email email, string? phone , Address address)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Customer name is required");

            if (name.Trim().Length > 150)
                throw new DomainException("Customer name cannot exceed 150 characters.");

            var cellPhone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();

            if(cellPhone is { Length: > 30})
                throw new DomainException("Phone cannot exceed 30 characters.");

            Name = name.Trim();
            Email = email ?? throw new DomainException("Email is required.");
            Phone = cellPhone;
            Address = address ?? throw new DomainException("Address is required.");
        }
        
    }
}
