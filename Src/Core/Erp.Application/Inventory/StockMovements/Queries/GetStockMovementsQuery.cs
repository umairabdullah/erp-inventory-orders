using Erp.Application.Common.Exceptions;
using Erp.Application.Common.Models;
using Erp.Application.Inventory.Products;
using FluentValidation;
using MediatR;

namespace Erp.Application.Inventory.StockMovements.Queries
{
    public sealed record GetStockMovementsQuery(Guid ProductId, int Page = 1, int PageSize = 20) : IRequest<PagedResult<StockMovementDto>>;

    public sealed class GetStockMovementsQueryValidator : AbstractValidator<GetStockMovementsQuery>
    {
        public GetStockMovementsQueryValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
        }

    }

    public sealed class GetStockMovementsQueryHandler : IRequestHandler<GetStockMovementsQuery, PagedResult<StockMovementDto>>
    {
        private readonly IStockMovementQueries _movements;
        private readonly IProductQueries _product;

        public GetStockMovementsQueryHandler(IStockMovementQueries movements, IProductQueries product)
        {
            _movements = movements;
            _product = product;
        }

        public async Task<PagedResult<StockMovementDto>> Handle(GetStockMovementsQuery request, CancellationToken cancellationToken)
        {
            if(await _product.GetByIdAsync(request.ProductId, cancellationToken) is null)
                throw new NotFoundException("Product", request.ProductId);

            var page = Math.Max(1, request.Page);
            var pageSize = Math.Clamp(request.PageSize, 1, 100);

            return await _movements.GetPagedProductAsync(request.ProductId, page, pageSize, cancellationToken);
        }
    }
}
