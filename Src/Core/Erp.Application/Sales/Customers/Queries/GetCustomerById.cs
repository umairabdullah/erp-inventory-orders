using Erp.Application.Common.Exceptions;
using MediatR;

namespace Erp.Application.Sales.Customers.Queries
{
    public sealed record GetCustomerByIdQuery(Guid Id) : IRequest<CustomerDto>;

    public sealed class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerDto>
    {
        private readonly ICustomerQueries _queries;
        public GetCustomerByIdQueryHandler(ICustomerQueries queries) => _queries = queries;
        public async Task<CustomerDto> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
            => await _queries.GetByIdAsync(request.Id, cancellationToken) ?? throw new NotFoundException("Customer", request.Id);
    }
}
