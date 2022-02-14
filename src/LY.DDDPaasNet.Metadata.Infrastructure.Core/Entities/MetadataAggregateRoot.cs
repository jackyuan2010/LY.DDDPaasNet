using LY.DDDPaasNet.Domain.Abstractions.Entities;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace LY.DDDPaasNet.Metadata.Infrastructure.Core.Entities;

public abstract class MetadataAggregateRoot : Entity, IMetadataAggregateRoot
{
    #region DomainEvent
    private readonly ICollection<IDomainEvent> domainEvents = new Collection<IDomainEvent>();

    public IReadOnlyCollection<IDomainEvent> DomainEvents => domainEvents.ToImmutableList();

    protected virtual void AddDomainEvent(IDomainEvent eventItem)
    {
        domainEvents.Add(eventItem);
    }

    public virtual void RemoveDomainEvent(IDomainEvent eventItem)
    {
        domainEvents.Remove(eventItem);
    }

    public virtual void ClearDomainEvent()
    {                                                                                        
        domainEvents.Clear();
    }
    #endregion DomainEvent
}

public abstract class MetadataAggregateRoot<TKey> : Entity<TKey>, IMetadataAggregateRoot<TKey>
{
    #region DomainEvent
    private readonly ICollection<IDomainEvent> domainEvents = new Collection<IDomainEvent>();

    public IReadOnlyCollection<IDomainEvent> DomainEvents => domainEvents.ToImmutableList();

    protected virtual void AddDomainEvent(IDomainEvent eventItem)
    {
        domainEvents.Add(eventItem);
    }

    public virtual void RemoveDomainEvent(IDomainEvent eventItem)
    {
        domainEvents.Remove(eventItem);
    }

    public virtual void ClearDomainEvent()
    {
        domainEvents.Clear();
    }
    #endregion DomainEvent
}