using Erp.Application.Inventory.StockMovements.Commands;
using Erp.Domain.Inventory;
using FluentValidation;

namespace Erp.Application.Inventory.StockMovements
{
    public sealed record StockMovementDto(Guid Id, Guid ProductId, StockMovementType Type, int QuantityChange, int BalanceAfter, string? Reason, Guid? PerformedByUserId, DateTime CreatedAtUtc);
   
   

   
}
