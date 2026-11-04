using Erp.Api.Authorization;
using Erp.Application.Inventory.Users.Commands;
using Erp.Application.Inventory.Users.Queries;
using Erp.Domain.Identity.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ISender _sender;
        public UsersController(ISender sender) => _sender = sender;

        [HttpGet]
        [Authorize(Roles = AppRole.ManagerOrAdmin)]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
            => Ok(await _sender.Send(new GetUsersQuery(search, page, pageSize), cancellationToken));

        [HttpPut("{id:guid}/role")]
        [Authorize(Roles = AppRole.ManagerOrAdmin)]
        public async Task<IActionResult> ChangeRole(Guid id, ChangeUserRoleRequest request, CancellationToken cancellationToken)
        {
            await _sender.Send(new ChangeUserRoleCommand(id, request.Role), cancellationToken);
            return NoContent();
        }
    }
    public sealed record ChangeUserRoleRequest(Role Role);
}
