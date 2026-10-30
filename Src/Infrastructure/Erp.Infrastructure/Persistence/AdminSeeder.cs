using Erp.Application.Common.Interfaces;
using Erp.Domain.Common;
using Erp.Domain.Identity;
using Erp.Domain.Identity.Repositories;
using Erp.Domain.ValueObjects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Erp.Infrastructure.Persistence
{
    public static class AdminSeeder
    {
        public static async Task SeedAsync(IServiceProvider services, CancellationToken cancellationToken = default) 
        {
            using var scope = services.CreateScope();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var emailValue = config["Seed:AdminEmail"];
            var password = config["Seed:AdminPassword"];

            if (string.IsNullOrWhiteSpace(emailValue) || string.IsNullOrWhiteSpace(password))
                return;

            var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var email = Email.Create(emailValue);
            if (await users.EmailExistsAsync(email, cancellationToken))
                return;

            var admin = User.Register("Administrator", email, hasher.Hash(password), Role.Admin);
            admin.ConfirmEmail();

            await users.AddAsync(admin, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

        }
    }
}
