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

        
        [HttpPost("{id:guid}/confirm")]
        public async Task<IActionResult> Confirm(Guid id, CancellationToken cancellationToken)
        {
            await _sender.Send(new ChangeOrderStatusCommand(id, OrderStatus.Confirmed), cancellationToken);
            return NoContent();
        }


        [HttpPost("{id:guid}/ship")]
        public async Task<IActionResult> Ship(Guid id, CancellationToken cancellationToken)
        {
            await _sender.Send(new ChangeOrderStatusCommand(id, OrderStatus.Shipped), cancellationToken);
            return NoContent();
        }

        [HttpPost("{id:guid}/deliver")]
        public async Task<IActionResult> Deliver(Guid id, CancellationToken cancellationToken)
        {
            await _sender.Send(new ChangeOrderStatusCommand(id, OrderStatus.Delivered), cancellationToken);
            return NoContent();
        }


        [HttpPost("{id:guid}/cancel")]
        public async Task<IActionResult> Cancel(Guid id, CancelOrderRequest? request, CancellationToken cancellationToken)
        {
            await _sender.Send(new CancelOrderCommand(id, request?.Reason), cancellationToken);
            return NoContent();
        }

        public sealed record CancelOrderRequest(string? Reason);
    }
}
