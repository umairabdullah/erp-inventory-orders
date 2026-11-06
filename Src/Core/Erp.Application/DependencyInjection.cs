using Erp.Application.Auth;
using Erp.Application.Common.Behaviors;
using Erp.Application.Common.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Erp.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = typeof(DependencyInjection).Assembly;

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
            services.AddValidatorsFromAssembly(assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviors<,>));
            services.AddScoped<AuthSessionIssuer>();
            services.AddScoped<EmailConfirmationSender>();

            var handlerInterface = typeof(IDomainEventHandler<>);
            var handlerTypes = assembly.GetTypes().Where(t => t is { IsClass: true, IsAbstract: false });

            foreach(var type in handlerTypes)
            {
                foreach(var implemented in type.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterface))
                {
                    services.AddScoped(implemented, type);
                }
            }


            return services;
        }
    }
}
