using Erp.Application.Common.Models;
using MediatR;

namespace Erp.Application.Inventory.Categories.Queries
{
    public sealed record GetCategoriesQuery(string? Search, int Page = 1, int PageSize = 10) : IRequest<PagedResult<CategoryDto>>;
    public sealed class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, PagedResult<CategoryDto>>
    {
        private readonly ICategoryQueries _queries;
        public GetCategoriesQueryHandler(ICategoryQueries queries) 
        {
            _queries = queries;
        }

        public Task<PagedResult<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var page = Math.Max(1, request.Page);
            var pageSize = Math.Clamp(request.PageSize, 1, 100);

            return _queries.GetPagedAsync(request.Search, page, pageSize, cancellationToken);
        }
    }
}
