using Erp.Application.Common.Exceptions;
using Erp.Domain.Common;
using Erp.Domain.Inventory.Repositories;
using Erp.Domain.ValueObjects;
using FluentValidation;
using MediatR;
using System.Runtime.InteropServices;

namespace Erp.Application.Inventory.Products.Commands
{
    public sealed record UpdateProductCommand(
        Guid Id,
        string Name,
        string? Description,
        decimal Price,
        string Currency,
        int ReorderLevel,
        Guid CategoryId) : IRequest;

    public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {

        public UpdateProductCommandValidator()
        {
            RuleFor(p => p.Id).NotEmpty();
            RuleFor(p => p.Name).NotEmpty().MaximumLength(200);
            RuleFor(p => p.Description).MaximumLength(2000);
            RuleFor(p => p.Price).GreaterThanOrEqualTo(0);
            RuleFor(p => p.Currency).NotEmpty().Length(3);
            RuleFor(p => p.ReorderLevel).GreaterThanOrEqualTo(0);
            RuleFor(p => p.CategoryId).NotEmpty();
        }
    }

    public sealed class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>
    {
        private readonly IProductRepository _products;
        private readonly ICategoryRepository _categories;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProductCommandHandler(IProductRepository products, ICategoryRepository categories, IUnitOfWork unitOfWork)
        {
            _products = products;
            _categories = categories;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _products.GetByIdAsync(request.Id, cancellationToken) ?? throw new NotFoundException("Product", request.Id);

            var category = await _categories.GetByIdAsync(request.CategoryId, cancellationToken) ?? throw new NotFoundException("Category", request.CategoryId);

            var price = Money.Create(request.Price, request.Currency);

            product.UpdateDetails(request.Name, request.Description, price, request.ReorderLevel, category.Id);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
