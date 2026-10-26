using Erp.Application.Common.Exceptions;
using MediatR;

namespace Erp.Application.Inventory.Products.Queries
{
    public sealed record GetProductByIdQuery(Guid Id) : IRequest<ProductDto>;

    public sealed class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
    {
        private readonly IProductQueries _queries;
        public GetProductByIdQueryHandler(IProductQueries queries) => _queries = queries;

        public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            return await _queries.GetByIdAsync(request.Id, cancellationToken) ?? throw new NotFoundException("Product", request.Id);
        }

       
       
    }
}
