using LY.DDDPaasNet.EntityFrameworkCore.Repositories;
using LY.DDDPaasNet.Metadata.Infrastructure.Core.Entities;

namespace LY.DDDPaasNet.Metadata.Infrastructure.Core.Repositories;

public interface IMetadataRepository<TEntity> : IEFCoreRepository<TEntity>
    where TEntity : class, IMetadataAggregateRoot
{
}

public interface IMetadataRepository<TEntity, TKey> : IMetadataRepository<TEntity>, IEFCoreRepository<TEntity, TKey>
    where TEntity : class, IMetadataAggregateRoot<TKey>
{
}