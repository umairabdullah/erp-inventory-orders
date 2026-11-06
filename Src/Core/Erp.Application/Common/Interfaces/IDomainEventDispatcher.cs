using Erp.Domain.Common;

namespace Erp.Application.Common.Interfaces
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken cancellationToken);
    }
}
