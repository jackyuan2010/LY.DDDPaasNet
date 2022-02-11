using LY.DDDPaasNet.Core.DependencyInjection;
using LY.DDDPaasNet.Domain.Abstractions.Uow;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace LY.DDDPaasNet.EntityFrameworkCore;

public abstract class EFCoreDbContext<TDbContext> : DbContext, Domain.Abstractions.Uow.IDatabase, ITransaction
    where TDbContext : DbContext
{
    private IDbContextTransaction? dbContextTransaction;

    public bool HasActiveTransactin => dbContextTransaction != null;

    public ILazyServiceProvider LazyServiceProvider { get; }

    public IUnitOfWorkManager UnitOfWorkManager => LazyServiceProvider.LazyGetRequiredService<IUnitOfWorkManager>();

    public ILogger<EFCoreDbContext<TDbContext>> Logger => LazyServiceProvider.LazyGetService<ILogger<EFCoreDbContext<TDbContext>>>(NullLogger<EFCoreDbContext<TDbContext>>.Instance);

    protected EFCoreDbContext(DbContextOptions<TDbContext> options, ILazyServiceProvider lazyServiceProvider)
        : base(options)
    {
        LazyServiceProvider = lazyServiceProvider;
    }

    public virtual async Task<IDbContextTransaction> BeginTranscationAsync(CancellationToken cancellationToken = default)
    {
        if (dbContextTransaction == null)
        {
            dbContextTransaction = await Database.BeginTransactionAsync(cancellationToken);
        }
        return dbContextTransaction;
    }

    public virtual async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (dbContextTransaction == null)
            return;

        try
        {
            await dbContextTransaction.CommitAsync(cancellationToken);
        }
        catch
        {
            throw;
        }
        finally
        {
            if (dbContextTransaction != null)
            {
                dbContextTransaction.Dispose();
                dbContextTransaction = null;
            }
        }
    }

    public virtual async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (dbContextTransaction == null)
            return;

        try
        {
            await dbContextTransaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            if (dbContextTransaction != null)
            {
                dbContextTransaction.Dispose();
                dbContextTransaction = null;
            }
        }
    }
}