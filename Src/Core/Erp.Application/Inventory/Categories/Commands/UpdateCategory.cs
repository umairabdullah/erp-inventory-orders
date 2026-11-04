using Erp.Application.Common.Exceptions;
using Erp.Domain.Common;
using Erp.Domain.Inventory.Repositories;
using FluentValidation;
using MediatR;

namespace Erp.Application.Inventory.Categories.Commands
{
    public sealed record UpdateCategoryCommand(Guid Id, string Name, string? Description) : IRequest;

    public sealed class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
    {
        public UpdateCategoryCommandValidator()
        { 
            RuleFor(c => c.Id).NotEmpty();
            RuleFor(c => c.Name).NotEmpty().MaximumLength(100);
            RuleFor(c => c.Description).MaximumLength(500);
        }
    }

    public sealed class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand>
    {
        private readonly ICategoryRepository _categories;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCategoryCommandHandler(ICategoryRepository categories, IUnitOfWork unitOfWork)
        {
            _categories = categories;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _categories.GetByIdAsync(request.Id, cancellationToken) ?? throw new NotFoundException("Category", request.Id);

            if (await _categories.NameExistsAsync(request.Name, category.Id, cancellationToken)) 
                throw new ConflictException($"A category name '{request.Name.Trim()}' already exists");

            category.Update(request.Name, request.Description);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

        }
    }
}
