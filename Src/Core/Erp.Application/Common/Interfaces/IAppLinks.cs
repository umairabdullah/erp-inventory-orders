namespace Erp.Application.Common.Interfaces
{
    public interface IAppLinks
    {
        string BuildEmailConfirmationLink(string email, string token);
    }
}
