using Erp.Application.Common.Models;
using MediatR;

namespace Erp.Application.Inventory.Users.Queries
{
    public sealed record GetUsersQuery(string? Search, int Page = 1, int PageSize = 10) : IRequest<PagedResult<UserListItemDto>>;

    public sealed class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PagedResult<UserListItemDto>>
        {
        private readonly IUserQueries _queries;
        public GetUsersQueryHandler(IUserQueries queries) => _queries = queries;


        Task<PagedResult<UserListItemDto>> IRequestHandler<GetUsersQuery, PagedResult<UserListItemDto>>.Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var page = Math.Max(1, request.Page);
            var pageSize = Math.Clamp(request.PageSize, 1, 100);

            return _queries.GetPagedAsync(request.Search, page, pageSize, cancellationToken);
        }
    }
}
