using Erp.Application.Common.Exceptions;
using MediatR;

namespace Erp.Application.Sales.Orders.Queries
{
    public sealed record GetOrderByIdQuery(Guid Id) : IRequest<OrderDto>;
    public sealed class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto>
    {
        private readonly IOrderQueries _queries;
        public GetOrderByIdQueryHandler(IOrderQueries queries) => _queries = queries;

        public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
            => await _queries.GetByIdAsync(request.Id, cancellationToken) ?? throw new NotFoundException("Order", request.Id);
    }
}
