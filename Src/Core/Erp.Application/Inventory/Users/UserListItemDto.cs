namespace Erp.Application.Inventory.Users
{
    public sealed record UserListItemDto(Guid Id, string FullName, string Email, string Role, bool IsActive, bool EmailConfirmed);
}
