using LY.DDDPaasNet.Core.DependencyInjection;
using LY.DDDPaasNet.EntityFrameworkCore.Repositories;
using LY.DDDPaasNet.Metadata.Infrastructure.Core.Entities;
using LY.DDDPaasNet.Metadata.Infrastructure.Core.EntityFrameworkCore;

namespace LY.DDDPaasNet.Metadata.Infrastructure.Core.Repositories;

public class MetadataRepository<TDbContext, TEntity> : EFCoreRepository<MetadataDbContext, IMetadataAggregateRoot> , IMetadataRepository
{
    public MetadataRepository(MetadataDbContext dbContext, ILazyServiceProvider lazyServiceProvider)
        : base(dbContext, lazyServiceProvider)
    {
    }
}

public class MetadataRepository<TDbContext, TEntity, TKey> : EFCoreRepository<MetadataDbContext, IMetadataAggregateRoot<TKey>>
{
    public MetadataRepository(MetadataDbContext dbContext, ILazyServiceProvider lazyServiceProvider)
        : base(dbContext, lazyServiceProvider)
    {
    }
}