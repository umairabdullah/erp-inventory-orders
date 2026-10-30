using Erp.Api.Authorization;
using Erp.Application.Inventory.Categories.Commands;
using Erp.Application.Inventory.Categories.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ISender _sender;
        public CategoriesController(ISender sender) => _sender = sender;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
        {
            return Ok(await _sender.Send(new GetCategoriesQuery(search, page, pageSize), cancellationToken));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            return Ok(await _sender.Send(new GetCategoryByIdQuery(id), cancellationToken));
        }

        [HttpPost]
        [Authorize(Roles = AppRole.ManagerOrAdmin)]
        public async Task<IActionResult> Create(CreateCategoryCommand createCategoryCommand, CancellationToken cancellationToken = default) 
        {
            var id = await _sender.Send(createCategoryCommand, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }
    }
}
