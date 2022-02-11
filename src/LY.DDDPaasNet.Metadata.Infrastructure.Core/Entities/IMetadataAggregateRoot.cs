using LY.DDDPaasNet.Domain.Abstractions.Entities;

namespace LY.DDDPaasNet.Metadata.Infrastructure.Core.Entities;

public interface IMetadataAggregateRoot : IEntity, IAggregateRoot
{
}

public interface IMetadataAggregateRoot<TKey> : IEntity<TKey>, IMetadataAggregateRoot
{
}