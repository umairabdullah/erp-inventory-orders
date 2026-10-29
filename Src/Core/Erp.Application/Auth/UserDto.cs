using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Application.Auth
{
    public sealed record UserDto(Guid Id, string FullName, string Email, string Role);


}