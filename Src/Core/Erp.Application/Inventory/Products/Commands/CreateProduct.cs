using Erp.Application.Common.Exceptions;
using Erp.Domain.Common;
using Erp.Domain.Inventory;
using Erp.Domain.Inventory.Repositories;
using Erp.Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace Erp.Application.Inventory.Products.Commands
{
    public sealed record CreateProductCommand(string Name, string? Description, string Sku, decimal Price,        string Currency, int InitialStock,int ReorderLevel, Guid CategoryId) : IRequest<Guid>;

    public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Description).MaximumLength(2000);
            RuleFor(x => x.Sku).NotEmpty().MaximumLength(30);
            RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Currency).NotEmpty().MaximumLength(3);
            RuleFor(x => x.InitialStock).GreaterThanOrEqualTo(0);
            RuleFor(x => x.ReorderLevel).GreaterThanOrEqualTo(0);
            RuleFor(x => x.CategoryId).NotEmpty();
        }

    }

    public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly IProductRepository _products;
        private readonly ICategoryRepository _categories;
        private readonly IStockMovementRepository _movements;
        private readonly IUnitOfWork _unitOfWork;

        public CreateProductCommandHandler(IProductRepository products, ICategoryRepository categories, IStockMovementRepository movements, IUnitOfWork unitOfWork)
        {
            _products = products;
            _categories = categories;
            _movements = movements;
            _unitOfWork = unitOfWork;
        }



        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var category = await _categories.GetByIdAsync(request.CategoryId, cancellationToken) ?? throw new NotFoundException("Cateogyr", request.CategoryId);

            var sku = Sku.Create(request.Sku);

            if (await _products.SkuExistsAsync(sku, cancellationToken))
                throw new ConflictException($"A product with Sku '{sku}' is already exists.");

            var price = Money.Create(request.Price, request.Currency);

            var product = Product.Create(request.Name, request.Description, sku, price, request.InitialStock, request.ReorderLevel, category.Id);

            await _products.AddAsync(product);

            if(request.InitialStock > 0)
            {
                var movement = StockMovement.RecordIn(product.Id, StockMovementType.Initial, request.InitialStock, product.QuantityInStock, "Initial Stock", null);

                await _movements.AddAsync(movement, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return product.Id;

        }
    }
}
