using Erp.Application.Common.Interfaces;
using Erp.Application.Inventory.Categories;
using Erp.Application.Inventory.Categories.Queries;
using Erp.Application.Inventory.Products;
using Erp.Application.Inventory.StockMovements;
using Erp.Domain.Common;
using Erp.Domain.Identity.Repositories;
using Erp.Domain.Inventory.Repositories;
using Erp.Infrastructure.Identity;
using Erp.Infrastructure.Persistence;
using Erp.Infrastructure.Persistence.Queries;
using Erp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;

namespace Erp.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");

            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
            services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

            //write side: Repositories
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IStockMovementRepository, StockMovementRepository>();

            //Read side: Queries
            services.AddScoped<ICategoryQueries, CategoryQueries>();
            services.AddScoped<IProductQueries, ProductQueries>();
            services.AddScoped<IStockMovementQueries,  StockMovementQueries>();

            services.AddScoped<ICategoryQueries, CategoryQueries>();
            services.AddScoped<IUserRepository, UserRepository>();
            
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddSingleton<ITokenServices, JwtTokenService>();

            return services;
        }
    }
}
