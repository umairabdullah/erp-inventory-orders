using Erp.Application.Sales.Orders.Commands;
using Erp.Application.Sales.Orders.Queries;
using Erp.Domain.Sales.Orders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly ISender _sender;
        public OrdersController(ISender sender) => _sender = sender;

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] OrderStatus? status,
            [FromQuery] Guid? customerId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)

            => Ok(await _sender.Send(new GetOrdersQuery(search, status, customerId, page, pageSize), cancellationToken));

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
            => Ok(await _sender.Send(new GetOrderByIdQuery(id),cancellationToken));

        [HttpPost]
        public async Task<IActionResult> Place (PlaceOrderCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
    }
}
