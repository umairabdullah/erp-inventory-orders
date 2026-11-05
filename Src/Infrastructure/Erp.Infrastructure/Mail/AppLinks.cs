using Erp.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Erp.Infrastructure.Mail
{
    public sealed class AppLinks : IAppLinks
    {
        private readonly string _baseUrl;
        public AppLinks(IConfiguration configuration) => _baseUrl = (configuration["App:FrontendBaseUrl"] ?? "http://localhost:5173").TrimEnd('/');
        public string BuildEmailConfirmationLink(string email, string token)
            => $"{_baseUrl}/confirm-email?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";
    }
}
