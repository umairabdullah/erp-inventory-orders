using Erp.Application.Common.Exceptions;
using MediatR;

namespace Erp.Application.Inventory.Categories.Queries
{
    public sealed record GetCategoryByIdQuery(Guid Id) : IRequest<CategoryDto>;

    public sealed class GetCategoryByIdHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
    {
        private readonly ICategoryQueries _queries;
        public GetCategoryByIdHandler(ICategoryQueries queries) => _queries = queries;

        public async Task<CategoryDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            return await _queries.GetByIdAsync(request.Id, cancellationToken) ?? throw new NotFoundException("Category", request.Id);
        }
    }
}
