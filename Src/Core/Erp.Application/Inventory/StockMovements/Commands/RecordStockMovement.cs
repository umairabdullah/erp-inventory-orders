using Erp.Application.Common.Exceptions;
using Erp.Domain.Common;
using Erp.Domain.Exceptions;
using Erp.Domain.Inventory;
using Erp.Domain.Inventory.Repositories;
using FluentValidation;
using MediatR;

namespace Erp.Application.Inventory.StockMovements.Commands
{
    public sealed record RecordStockMovementCommand(Guid ProductId, StockMovementType Type, int Quantity, string? Reason) : IRequest<StockLevelDto>;

    public sealed record StockLevelDto(Guid ProductId, int QuantityInStock, bool IsLowStock);

    public sealed class RecordStockMovementValidator : AbstractValidator<RecordStockMovementCommand>
    {
        public RecordStockMovementValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.Type).IsInEnum();
            RuleFor(x => x.Quantity).NotEqual(0);

            RuleFor(x => x.Quantity).GreaterThan(0)
                .When(x => x.Type != StockMovementType.Adjustment)
                .WithMessage("Quantity must be greater than 0 for In and Out movements.");

            RuleFor(x => x.Reason).NotEmpty()
                .When(x => x.Type == StockMovementType.Adjustment)
                .WithMessage("A reason is required for stock adjustments.");

            RuleFor(x => x.Reason).MaximumLength(500);
        }

       
    }

    public sealed class RecordStockMovementCommandHandler : IRequestHandler<RecordStockMovementCommand, StockLevelDto>
    {
        private readonly IProductRepository _products;
        private readonly IStockMovementRepository _movements;
        private readonly IUnitOfWork _unitOfWork;

        public RecordStockMovementCommandHandler(IProductRepository products, IStockMovementRepository movements, IUnitOfWork unitOfWork)
        {
            _products = products;
            _movements = movements;
            _unitOfWork = unitOfWork;
        }


        public async Task<StockLevelDto> Handle(RecordStockMovementCommand request, CancellationToken cancellationToken)
        {
            var product = await _products.GetByIdAsync(request.ProductId, cancellationToken) ?? throw new NotFoundException("Product", request.ProductId);

            StockMovement movement;

            switch (request.Type)
            {

                case StockMovementType.In:
                    product.AddStock(request.Quantity);
                    movement = StockMovement.RecordIn(product.Id, StockMovementType.In, request.Quantity, product.QuantityInStock, request.Reason, null);
                    break;
                case StockMovementType.Out:
                    product.RemoveStock(request.Quantity);
                    movement = StockMovement.RecordOut(product.Id, StockMovementType.Out, request.Quantity, product.QuantityInStock, request.Reason, null);
                    break;
                case StockMovementType.Adjustment:
                    if (request.Quantity > 0)
                        product.AddStock(request.Quantity);
                    else
                        product.RemoveStock(-request.Quantity);
                    movement = StockMovement.RecordAdjustment(
                     product.Id, request.Quantity, product.QuantityInStock, request.Reason!, null);
                    break;
                default:
                    throw new DomainException("Unknown stock movement type.");
            }

            
            await _movements.AddAsync(movement, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new StockLevelDto(product.Id, product.QuantityInStock, product.IsLowStock);
        }
    }


}
