using Erp.Application.Inventory.Products.Commands;
using Erp.Application.Inventory.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ISender _sender;
        public ProductController(ISender sender) => _sender = sender;

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search, 
            [FromQuery] Guid? categoryId, 
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10, 
            CancellationToken cancellationToken = default)

            => Ok(await _sender.Send(new GetProductsQuery(search, categoryId, page, pageSize), cancellationToken));

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
            => Ok(await _sender.Send(new GetProductByIdQuery(id), cancellationToken));

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductCommand command, CancellationToken cancellationToken)
        {
            var id = await _sender.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }
    }
}
