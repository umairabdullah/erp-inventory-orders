using Erp.Application.Common.Models;
using MediatR;

namespace Erp.Application.Sales.Customers.Queries
{
    public sealed record GetCustomersQuery(string? Search, bool isActive, int page = 1, int pageSize = 10) : IRequest<PagedResult<CustomerDto>>;

    public sealed class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, PagedResult<CustomerDto>>
    {
        private readonly ICustomerQueries _queries;
        public GetCustomersQueryHandler(ICustomerQueries queries) => _queries = queries;

        public Task<PagedResult<CustomerDto>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
        {
            var page = Math.Max(1, request.page);
            var pageSize = Math.Clamp(request.pageSize, 1, 100);

            return _queries.GetPagedAsync(request.Search, request.isActive, page, pageSize, cancellationToken);
        }
    }
}
