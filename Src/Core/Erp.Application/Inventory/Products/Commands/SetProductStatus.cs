using Erp.Application.Common.Exceptions;
using Erp.Domain.Common;
using Erp.Domain.Inventory;
using Erp.Domain.Inventory.Repositories;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Application.Inventory.Products.Commands
{
    public sealed record SetProductStatusCommand(Guid Id, bool IsActive) : IRequest;

    public sealed class SetProductStatusCommandValidator : AbstractValidator<SetProductStatusCommand>
    {
        public SetProductStatusCommandValidator()
        {
            RuleFor(s => s.Id).NotEmpty();
        }
    }

    public sealed class SetProductStatusCommandHandler : IRequestHandler<SetProductStatusCommand>
    {
        private readonly IProductRepository _products;
        private readonly IUnitOfWork _unitOfWork;

        public SetProductStatusCommandHandler(IProductRepository products, IUnitOfWork unitOfWork)
        {
            _products = products;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(SetProductStatusCommand request, CancellationToken cancellationToken)
        {
            var product = await _products.GetByIdAsync(request.Id, cancellationToken) ?? throw new NotFoundException("Product", request.Id);

            if (request.IsActive)
                product.Activate();
            else
                product.Deactivate();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

        }
    }
}
