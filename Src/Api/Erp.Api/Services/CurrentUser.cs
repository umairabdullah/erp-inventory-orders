using Erp.Application.Common.Interfaces;

namespace Erp.Api.Services
{
    public sealed class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _accessor;
        public CurrentUser(IHttpContextAccessor accessor) => _accessor = accessor;

        public Guid? UserId 
        {
            get
            {
                var value = _accessor.HttpContext?.User.FindFirst("sub")?.Value;
                return Guid.TryParse(value, out var id) ? id : null;
            }
        }
    }
}
