using Erp.Api.Authorization;
using Erp.Application.Sales.Customers.Commands;
using Erp.Application.Sales.Customers.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly ISender _sender;
        public CustomersController(ISender sender) => _sender = sender;

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] bool isActive,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
                => Ok(await _sender.Send(new GetCustomersQuery(search, isActive, page, pageSize), cancellationToken));

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
            => Ok(await _sender.Send(new GetCustomerByIdQuery(id), cancellationToken));

        [HttpPost]
        public async Task<IActionResult> Create(CreateCustomerCommand command , CancellationToken cancellationToken)
        {
            var id = await _sender.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateCustomerCommand command, CancellationToken cancellationToken)
        {
            await _sender.Send(command with { Id = id }, cancellationToken);
            return NoContent();
        }

        [HttpPatch("{id:guid}/status")]
        [Authorize(Roles = AppRole.ManagerOrAdmin)]
        public async Task<IActionResult> SetStatus (Guid id, SetCustomerStatusRequest request, CancellationToken cancellationToken)
        {
            await _sender.Send(new SetCustomerStatusCommand(id, request.IsActive), cancellationToken);
            return NoContent();
        }
    }

    public sealed record SetCustomerStatusRequest(bool IsActive);
}
