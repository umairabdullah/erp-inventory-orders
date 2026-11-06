namespace Erp.Domain.Common
{
    public interface IDomainEvent
    {
        DateTime OccuredAtUtc { get; }
    }
}
