using Erp.Application.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Erp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ISender _sender;
        public AuthController(ISender sender) => _sender = sender;


        [HttpPost("register")]
        [EnableRateLimiting("login")]
        public async Task<IActionResult> Register(RegisterCommand command, CancellationToken cancellationToken)
        {
            var id = await _sender.Send(command, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, new { id });
        }

        [HttpPost("login")]
        [EnableRateLimiting("login")]
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

        [HttpPost]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailCommand command, CancellationToken cancellationToken)
        {
            await _sender.Send(command, cancellationToken);
            return NoContent();
        }

        [HttpPost("resend-confirmation")]
        [EnableRateLimiting("login")]
        public async Task<IActionResult> ResendConfirmation(ResendConfirmationCommand command, CancellationToken cancellationToken)
        {
            await _sender.Send(command, cancellationToken);
            return NoContent();
        }
    }
}
