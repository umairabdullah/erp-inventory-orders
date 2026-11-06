using Erp.Application.Common.Interfaces;
using Erp.Domain.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Erp.Infrastructure.Persistence
{
    public sealed class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<DomainEventDispatcher> _logger;

        public DomainEventDispatcher(IServiceProvider services, ILogger<DomainEventDispatcher> logger)
        {
            _services = services;
            _logger = logger;
        }

        public async Task DispatchAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken cancellationToken)
        {
            foreach (var domainEvent in domainEvents) 
            { 
                var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
                var handleMethod = handlerType.GetMethod("Handle")!;

                foreach(var handler in _services.GetServices(handlerType))
                {
                    try
                    {
                        await (Task) handleMethod.Invoke(handler, new object[] { domainEvent, cancellationToken })!;
                    }
                    catch (OperationCanceledException)
                    {

                        throw;
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError(ex, "Handler {Handler} failed for event {Event}", handler!.GetType().Name, domainEvent.GetType().Name);
                    }
                }
            }
        }
    }
}
