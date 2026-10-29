using Erp.Application.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ISender _sender;
        public AuthController(ISender sender) => _sender = sender;

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterCommand command, CancellationToken cancellationToken)
        {
            var id = await _sender.Send(command, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, new { id });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginCommand command, CancellationToken cancellationToken)
            => Ok(await _sender.Send(command, cancellationToken));

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshSessionCommand command, CancellationToken cancellationToken) 
            => Ok (await _sender.Send(command, cancellationToken));

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(LogoutCommand command, CancellationToken cancellationToken)
        {
            await _sender.Send(command, cancellationToken);
            return NoContent();
        }
             
    }
}
