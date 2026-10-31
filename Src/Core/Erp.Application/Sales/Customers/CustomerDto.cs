namespace Erp.Application.Sales.Customers
{
    public sealed record CustomerDto(Guid Id, string Name, string Email, string? Phone, AddressDto Address, bool IsActive);

    public sealed record AddressDto(string Street, string City, string? State, string PostalCode, string Country);
}
