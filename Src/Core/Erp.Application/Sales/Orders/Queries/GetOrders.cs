using Erp.Application.Common.Models;
using Erp.Domain.Sales.Orders;
using MediatR;

namespace Erp.Application.Sales.Orders.Queries
{
    public sealed record  GetOrdersQuery(string? Search, OrderStatus? Status, Guid? CustomerId, int Page = 1, int PageSize = 10) : IRequest<PagedResult<OrderSummaryDto>>;

    public sealed class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, PagedResult<OrderSummaryDto>>
    {
        private readonly IOrderQueries _queries;
        public GetOrdersQueryHandler(IOrderQueries queries) => _queries = queries;

        public Task<PagedResult<OrderSummaryDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
        {
            var page = Math.Max(1, request.Page);
            var pageSize = Math.Clamp(request.PageSize, 1, 100);

            return _queries.GetPagedAsync(request.Search, request.Status, request.CustomerId, page, pageSize, cancellationToken);
        }
    }
}
