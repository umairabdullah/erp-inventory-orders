using Erp.Application.Inventory.Products.Queries;
using Erp.Application.Inventory.StockMovements.Commands;
using Erp.Application.Inventory.StockMovements.Queries;
using Erp.Domain.Inventory;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockMovementsController : ControllerBase
    {
        private readonly ISender _sender;
        public StockMovementsController(ISender sender) => _sender = sender;

        [HttpGet]
        public async Task<IActionResult> GetHistory([FromQuery] Guid productId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20 , CancellationToken cancellationToken = default) 

            => Ok(await _sender.Send(new GetStockMovementsQuery(productId, page, pageSize), cancellationToken));

        [HttpPost]
        public async Task<IActionResult> Record(
             RecordStockMovementRequest request, CancellationToken cancellationToken)
             => Ok(await _sender.Send(
                 new RecordStockMovementCommand(request.ProductId, request.Type, request.Quantity, request.Reason),
                 cancellationToken));

    }
    public sealed record RecordStockMovementRequest(
         Guid ProductId, StockMovementType Type, int Quantity, string? Reason);
}
