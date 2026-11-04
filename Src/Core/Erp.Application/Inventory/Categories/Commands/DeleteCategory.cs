using Erp.Application.Common.Exceptions;
using Erp.Domain.Common;
using Erp.Domain.Inventory.Repositories;
using FluentValidation;
using MediatR;

namespace Erp.Application.Inventory.Categories.Commands
{
    public sealed record DeleteCategoryCommand(Guid Id) : IRequest;

    public sealed class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
    {
        public DeleteCategoryCommandValidator()
        {
            RuleFor(d => d.Id).NotEmpty();
        }
    }

    public sealed class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand>
    {
        private readonly ICategoryRepository _categories;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCategoryCommandHandler(ICategoryRepository categories, IUnitOfWork unitOfWork)
        {
            _categories = categories;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _categories.GetByIdAsync(request.Id, cancellationToken) ?? throw new NotFoundException("Category", request.Id);

            if (await _categories.HasProductAsync(category.Id, cancellationToken)) throw new ConflictException("This category still has products. Move or deactivate them before deleting it.");

            _categories.Remove(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

}
