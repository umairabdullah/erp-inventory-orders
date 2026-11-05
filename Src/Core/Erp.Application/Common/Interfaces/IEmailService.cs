namespace Erp.Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendAsync(string toAddress, string toName, string subject, string htmlBody, CancellationToken cancellationToken = default);
    }
}
