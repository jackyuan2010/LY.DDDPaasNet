using LY.DDDPaasNet.EntityFrameworkCore.Repositories;
using LY.DDDPaasNet.Metadata.Infrastructure.Core.Entities;

namespace LY.DDDPaasNet.Metadata.Infrastructure.Core.Repositories;

public interface IMetadataRepository : IEFCoreRepository<IMetadataAggregateRoot>
{
}

public interface IMetadataRepository<TEntity, TKey> : IMetadataRepository, IEFCoreRepository<IMetadataAggregateRoot<TKey>>
{
}