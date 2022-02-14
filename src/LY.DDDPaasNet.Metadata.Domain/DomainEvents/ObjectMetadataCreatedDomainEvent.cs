using LY.DDDPaasNet.Metadata.Domain.Entities;
using LY.DDDPaasNet.Metadata.Infrastructure.Core.Entities;

namespace LY.DDDPaasNet.Metadata.Domain.DomainEvents;

public class ObjectMetadataCreatedDomainEvent : IDomainEvent
{
    public ObjectMetadata ObjectMetadata { get; private set; }

    public ObjectMetadataCreatedDomainEvent(ObjectMetadata order)
    {
        ObjectMetadata = order;
    }
}