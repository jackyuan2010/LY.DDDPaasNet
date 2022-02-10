using LY.DDDPaasNet.Core.DependencyInjection;
using LY.DDDPaasNet.Core.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace LY.DDDPaasNet.EntityFrameworkCore.Repositories;

public abstract class EFCoreRepository<TDbContext, TEntity> : RepositoryBase<TEntity>, IEFCoreRepository<TEntity>
    where TDbContext : EFCoreDbContext<TDbContext>
    where TEntity : class, IEntity
{
    protected virtual TDbContext DbContext { get; }

    public ILogger<EFCoreRepository<TDbContext, TEntity>> Logger { get; set; }

    public EFCoreRepository(TDbContext dbContext, ILazyServiceProvider lazyServiceProvider)
        : base(lazyServiceProvider)
    {
        DbContext = dbContext;
        Logger = NullLogger<EFCoreRepository<TDbContext, TEntity>>.Instance;
    }

    public virtual DbSet<TEntity> DbSet => DbContext.Set<TEntity>();

    protected virtual async Task BeginOperation(CancellationToken cancellationToken = default)
    {
        if (UnitOfWorkManager == null || UnitOfWorkManager.Current == null)
        {
            return;
        }
        try
        {
            var unitOfWork = UnitOfWorkManager.Current;
            var connectionString = DbContext.Database.GetDbConnection().ConnectionString;
            var databaseKey = $"Database_{connectionString}"; ;

            var database = unitOfWork.GetDatabase(databaseKey);

            if (database == null)
            {
                unitOfWork.AddDatabase(databaseKey, DbContext);
            }

            if (UnitOfWorkManager.Current.Options.IsTransactional)
            {
                var transactionKey = $"Transaction_{connectionString}";
                var activeTransaction = unitOfWork.GetTransaction(transactionKey);

                if (activeTransaction == null)
                {
                    await DbContext.BeginTranscationAsync(cancellationToken);

                    unitOfWork.AddTransaction(transactionKey, DbContext);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
        }
    }

    public override async Task<TEntity> InsertAsync(TEntity entity, 
        bool autoSave = false, 
        CancellationToken cancellationToken = default)
    {
        await BeginOperation(cancellationToken);

        var savedEntity = (await DbContext.Set<TEntity>().AddAsync(entity, cancellationToken)).Entity;

        if (autoSave)
        {
            await DbContext.SaveChangesAsync(cancellationToken);
        }

        return savedEntity;
    }

    public override async Task InsertManyAsync(IEnumerable<TEntity> entities, 
        bool autoSave = false, 
        CancellationToken cancellationToken = default)
    {
        var entityArray = entities.ToArray();

        await BeginOperation(cancellationToken);

        await DbContext.Set<TEntity>().AddRangeAsync(entityArray, cancellationToken);

        if (autoSave)
        {
            await DbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public override async Task<TEntity> UpdateAsync(TEntity entity, 
        bool autoSave = false, 
        CancellationToken cancellationToken = default)
    {
        await BeginOperation(cancellationToken);

        DbContext.Attach(entity);

        var updatedEntity = DbContext.Update(entity).Entity;

        if (autoSave)
        {
            await DbContext.SaveChangesAsync(cancellationToken);
        }

        return updatedEntity;
    }

    public override async Task UpdateManyAsync(IEnumerable<TEntity> entities, 
        bool autoSave = false, 
        CancellationToken cancellationToken = default)
    {
        await BeginOperation(cancellationToken);

        DbContext.Set<TEntity>().UpdateRange(entities);

        if (autoSave)
        {
            await DbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public override async Task<bool> DeleteAsync(TEntity entity, 
        bool autoSave = false, 
        CancellationToken cancellationToken = default)
    {
        await BeginOperation(cancellationToken);

        DbContext.Set<TEntity>().Remove(entity);

        if (autoSave)
        {
            await DbContext.SaveChangesAsync(cancellationToken);
        }
        return true;
    }

    public override async Task DeleteManyAsync(IEnumerable<TEntity> entities, 
        bool autoSave = false, 
        CancellationToken cancellationToken = default)
    {
        await BeginOperation(cancellationToken);

        DbContext.RemoveRange(entities);

        if (autoSave)
        {
            await DbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public override async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, 
        bool autoSave = false, 
        CancellationToken cancellationToken = default)
    {
        await BeginOperation(cancellationToken);

        var entities = await DbSet
            .Where(predicate)
            .ToListAsync(cancellationToken);

        await DeleteManyAsync(entities, autoSave, cancellationToken);

        if (autoSave)
        {
            await DbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public override async Task<List<TEntity>> GetListAsync(bool includeDetails = false, 
        CancellationToken cancellationToken = default)
    {
        return includeDetails
            ? await (await WithDetailsAsync()).ToListAsync(cancellationToken)
            : await DbSet.ToListAsync(cancellationToken);
    }

    public override async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, 
        bool includeDetails = false, 
        CancellationToken cancellationToken = default)
    {
        return includeDetails
            ? await (await WithDetailsAsync()).Where(predicate).ToListAsync(cancellationToken)
            : await DbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public override async Task<long> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.LongCountAsync(cancellationToken);
    }

    public override async Task<List<TEntity>> GetPagedListAsync(int skipCount,
        int maxResultCount,
        string sorting,
        bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        var queryable = includeDetails
            ? await WithDetailsAsync()
            : DbSet;

        return await queryable.Skip(skipCount).Take(maxResultCount).ToListAsync(cancellationToken);
    }

    public override Task<IQueryable<TEntity>> GetQueryableAsync(CancellationToken cancellationToken = default)
    {
        if(cancellationToken.IsCancellationRequested)
        {
            throw new TaskCanceledException();
        }
        return Task.FromResult(DbSet.AsQueryable());
    }

    protected override async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await DbContext.SaveChangesAsync(cancellationToken);
    }

    public override async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate,
        bool includeDetails = true,
        CancellationToken cancellationToken = default)
    {
        return includeDetails
            ? await (await WithDetailsAsync(cancellationToken)).Where(predicate).SingleOrDefaultAsync(cancellationToken)
            : await DbSet.Where(predicate).SingleOrDefaultAsync(cancellationToken);
    }

    public virtual async Task EnsureCollectionLoadedAsync<TProperty>(
        TEntity entity,
        Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression,
        CancellationToken cancellationToken = default)
        where TProperty : class
    {
        await DbContext
            .Entry(entity)
            .Collection(propertyExpression)
            .LoadAsync(cancellationToken);
    }

    public virtual async Task EnsurePropertyLoadedAsync<TProperty>(
        TEntity entity,
        Expression<Func<TEntity, TProperty>> propertyExpression,
        CancellationToken cancellationToken = default)
        where TProperty : class
    {
        await DbContext
            .Entry(entity)
            .Reference(propertyExpression)
            .LoadAsync(cancellationToken);
    }

    public override async Task<IQueryable<TEntity>> WithDetailsAsync(CancellationToken cancellationToken = default)
    {
        return await GetQueryableAsync(cancellationToken);
    }

    public override async Task<IQueryable<TEntity>> WithDetailsAsync(CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] propertySelectors)
    {
        return IncludeDetails(await GetQueryableAsync(cancellationToken), propertySelectors);
    }

    private static IQueryable<TEntity> IncludeDetails(IQueryable<TEntity> query,
        Expression<Func<TEntity, object>>[] propertySelectors)
    {
        if (!propertySelectors.IsNullOrEmpty())
        {
            foreach (var propertySelector in propertySelectors)
            {
                query = query.Include(propertySelector);
            }
        }

        return query;
    }

}

public abstract class EFCoreRepository<TDbContext, TEntity, TKey> : EFCoreRepository<TDbContext, TEntity>,
    IEFCoreRepository<TEntity, TKey>
    where TDbContext : EFCoreDbContext<TDbContext>
    where TEntity : class, IEntity<TKey>
{
    public EFCoreRepository(TDbContext dbContext, ILazyServiceProvider lazyServiceProvider)
        : base(dbContext, lazyServiceProvider)
    {
    }

    public virtual async Task<TEntity> GetAsync(TKey id, 
        bool includeDetails = true, 
        CancellationToken cancellationToken = default)
    {
        var entity = await FindAsync(id, includeDetails, cancellationToken);

        if (entity == null)
        {
            throw new EntityNotFoundException(typeof(TEntity), id);
        }

        return entity;
    }

    public virtual async Task<TEntity> FindAsync(TKey id, 
        bool includeDetails = true, 
        CancellationToken cancellationToken = default)
    {
        return includeDetails
            ? await (await WithDetailsAsync()).OrderBy(e => e.Id).FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken)
            : await DbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public virtual async Task<bool> DeleteAsync(TKey id, 
        bool autoSave = false, 
        CancellationToken cancellationToken = default)
    {
        var entity = await FindAsync(id, cancellationToken: cancellationToken);
        if (entity == null)
        {
            return false;
        }

        await DeleteAsync(entity, autoSave, cancellationToken);
        return true;
    }

    public virtual async Task DeleteManyAsync(IEnumerable<TKey> ids, 
        bool autoSave = false, 
        CancellationToken cancellationToken = default)
    {
        var entities = await DbSet.Where(x => ids.Contains(x.Id)).ToListAsync(cancellationToken);

        await DeleteManyAsync(entities, autoSave, cancellationToken);
    }
}