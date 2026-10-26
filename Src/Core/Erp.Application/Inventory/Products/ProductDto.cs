namespace Erp.Application.Inventory.Products
{
    public sealed record ProductDto(
        Guid Id, 
        string Name, 
        string? Description,
        string Sku, 
        decimal PriceAmount, 
        string PriceCurrency, 
        int QuantityInStock, 
        int ReorderLevel, 
        bool IsLowStock, 
        bool IsActive, 
        Guid CategoryId, 
        string CategoryName);
}
