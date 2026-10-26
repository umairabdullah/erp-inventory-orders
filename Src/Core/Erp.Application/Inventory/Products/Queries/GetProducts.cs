using Erp.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Application.Inventory.Products.Queries
{
    public sealed record GetProductsQuery(string? Search, Guid? CategoryId, int Page = 1, int PageSize = 10) : IRequest<PagedResult<ProductDto>>;

    public sealed class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PagedResult<ProductDto>>
    {
        private readonly IProductQueries _queries;
        public GetProductsQueryHandler(IProductQueries queries) => _queries = queries;


        public Task<PagedResult<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var page = Math.Max(1, request.Page);
            var pageSize = Math.Clamp(request.PageSize, 1, 100);

            return _queries.GetPagedAsync(request.Search, request.CategoryId, page, pageSize, cancellationToken);
        }
    }
}
