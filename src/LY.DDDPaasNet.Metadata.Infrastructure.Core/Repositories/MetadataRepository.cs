using LY.DDDPaasNet.Core.DependencyInjection;
using LY.DDDPaasNet.EntityFrameworkCore.Repositories;
using LY.DDDPaasNet.Metadata.Infrastructure.Core.Entities;
using LY.DDDPaasNet.Metadata.Infrastructure.Core.EntityFrameworkCore;

namespace LY.DDDPaasNet.Metadata.Infrastructure.Core.Repositories;

public class MetadataRepository<TDbContext, TEntity> : EFCoreRepository<MetadataDbContext, TEntity> , IMetadataRepository<TEntity>
    where TEntity : class, IMetadataAggregateRoot
{
    public MetadataRepository(MetadataDbContext dbContext, ILazyServiceProvider lazyServiceProvider)
        : base(dbContext, lazyServiceProvider)
    {
    }
}

public class MetadataRepository<TDbContext, TEntity, TKey> : EFCoreRepository<MetadataDbContext, TEntity, TKey>, IMetadataRepository<TEntity, TKey>
    where TEntity : class, IMetadataAggregateRoot<TKey>
{
    public MetadataRepository(MetadataDbContext dbContext, ILazyServiceProvider lazyServiceProvider)
        : base(dbContext, lazyServiceProvider)
    {
    }
}