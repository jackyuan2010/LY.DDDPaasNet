namespace LY.DDDPaasNet.EntityFrameworkCore.Repositories;

public interface IEFCoreRepository<TEntity> : IRepository<TEntity>
    where TEntity : class, IEntity
{
    DbSet<TEntity> DbSet { get; }
}

public interface IEFCoreRepository<TEntity, TKey> : IEFCoreRepository<TEntity>, IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
}