using Erp.Application.Common.Exceptions;
using Erp.Domain.Common;
using Erp.Domain.Inventory;
using Erp.Domain.Inventory.Repositories;
using FluentValidation;
using MediatR;

namespace Erp.Application.Inventory.Categories.Commands
{

    public sealed record CreateCategoryCommand(string Name, string? Description) : IRequest<Guid>;

    public sealed class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(c => c.Name).NotEmpty().MaximumLength(100);
            RuleFor(c => c.Description).MaximumLength(500);
        }
    }
    public sealed class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Guid>
    {
        private readonly ICategoryRepository _categories;
        private readonly IUnitOfWork _unitOfWork;

        public CreateCategoryCommandHandler(ICategoryRepository categories, IUnitOfWork unitOfWork)
        {
            _categories = categories;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            if (await _categories.NameExistsAsync(request.Name, cancellationToken))
                throw new ConflictException($"A category name '{request.Name}' already exists.");

            var category = Category.Create(request.Name, request.Description);
            await _categories.AddAsync(category, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return category.Id;
        }
    }
}
