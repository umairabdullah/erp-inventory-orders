using Erp.Domain.Common;

namespace Erp.Application.Common.Interfaces
{
    public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
    {
        Task Handle(TEvent domainEvent, CancellationToken cancellationToken);
    }
}
