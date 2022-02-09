using LY.DDDPaasNet.Core.Extensions;
using Microsoft.Extensions.Options;
using System.Collections.Immutable;

namespace LY.DDDPaasNet.Domain.Abstractions.Uow;

public class UnitOfWork : IUnitOfWork
{
    public Guid UnitWorkId => Guid.NewGuid();

    public bool IsDisposed { get; private set; }

    public bool IsCompleted { get; private set; }

    public bool IsFailedWihtException { get; private set; }

    public IUnitOfWorkOptions Options { get; private set; }

    public IServiceProvider ServiceProvider { get; }

    protected List<Func<Task>> CompletedHandlers { get; } = new List<Func<Task>>();

    protected List<Func<Task>> DisposedHandlers { get; } = new List<Func<Task>>();

    protected List<Func<Exception, Task>> FailedHandlers { get; } = new List<Func<Exception, Task>>();

    private readonly Dictionary<string, IDatabase> databases;
    private readonly Dictionary<string, ITransaction> transactions;

    private Exception exception;
    private bool isCompleting;
    private bool isRolledback;

    public UnitOfWork(IServiceProvider serviceProvider,
        IOptions<IUnitOfWorkOptions> options)
    {
        ServiceProvider = serviceProvider;
        Options = options.Value;
        databases = new Dictionary<string, IDatabase>();
        transactions = new Dictionary<string, ITransaction>();
    }

    public virtual void Initialize(IUnitOfWorkOptions options)
    {
        if (Options != null)
        {
            throw new InvalidOperationException("This unit of work is already initialized before!");
        }

        Options = options;
    }

    #region IDatabase
    public virtual IDatabase GetDatabase(string key)
    {
        return databases.GetOrDefault(key);
    }

    public virtual void AddDatabase(string key, IDatabase database)
    {
        if (databases.ContainsKey(key))
        {
            throw new InvalidOperationException("There is already a database in this unit of work with given key: " + key);
        }

        databases.Add(key, database);
    }

    public virtual IDatabase GetOrAddDatabase(string key, Func<IDatabase> factory)
    {
        return databases.GetOrAdd(key, factory);
    }

    public virtual IReadOnlyList<IDatabase> GetAllActiveDatabases()
    {
        return databases.Values.ToImmutableList();
    }
    #endregion

    #region ITransaction
    public virtual ITransaction GetTransaction(string key)
    {
        ITransaction transaction;
        if (transactions.TryGetValue(key, out transaction))
        {
            return transaction;
        }
        return null;
    }

    public virtual void AddTransaction(string key, ITransaction transaction)
    {
        if (transactions.ContainsKey(key))
        {
            throw new InvalidOperationException("There is already a transaction in this unit of work with given key: " + key);
        }

        transactions.Add(key, transaction);
    }

    public virtual ITransaction GetOrAddTransaction(string key, Func<ITransaction> factory)
    {
        ITransaction transaction;
        if (transactions.TryGetValue(key, out transaction))
        {
            return transaction;
        }

        transaction = factory();
        transactions[key] = transaction;
        return transaction;
    }

    public virtual IReadOnlyList<ITransaction> GetAllActiveTransactions()
    {
        return transactions.Values.ToImmutableList();
    }

    protected virtual async Task CommitTransactionsAsync()
    {
        foreach (var transaction in GetAllActiveTransactions())
        {
            try
            {
                await transaction.CommitTransactionAsync();
            }
            catch
            {
            }
        }
    }

    protected virtual async Task RollbackAllAsync(CancellationToken cancellationToken)
    {
        foreach (var transaction in GetAllActiveTransactions())
        {
            try
            {
                await transaction.RollbackTransactionAsync(cancellationToken);
            }
            catch
            {
            }
        }
    }

    protected void DisposeTransactions()
    {
        foreach (var transaction in GetAllActiveTransactions())
        {
            try
            {
                transaction.Dispose();
            }
            catch
            {
            }
        }
    }
    #endregion

    public virtual void OnCompleted(Func<Task> handler)
    {
        CompletedHandlers.Add(handler);
    }

    public virtual void OnDisposed(Func<Task> handler)
    {
        DisposedHandlers.Add(handler);
    }

    public virtual void OnFailed(Func<Exception, Task> handler)
    {
        FailedHandlers.Add(handler);
    }

    public virtual async void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }
        IsDisposed = true;
        DisposeTransactions();

        if (!IsCompleted || IsFailedWihtException)
        {
            await OnFailedAsync();
        }

        await OnDisposedAsync();
    }

    public virtual async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (isRolledback)
        {
            return;
        }
        isRolledback = true;
        await RollbackAllAsync(cancellationToken);
    }

    public virtual async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (isRolledback)
        {
            return;
        }
        foreach (var database in GetAllActiveDatabases())
        {
            await database.SaveChangesAsync(cancellationToken);
        }
    }

    public virtual async Task CompleteAsync(CancellationToken cancellationToken = default)
    {
        if (isRolledback)
        {
            return;
        }
        if (IsCompleted || isCompleting)
        {
            throw new InvalidOperationException("Complete is called before!");
        }
        try
        {
            isCompleting = true;
            await SaveChangesAsync(cancellationToken);
            await CommitTransactionsAsync();
            await OnCompletedAsync();
            IsCompleted = true;
        }
        catch (Exception ex)
        {
            exception = ex;
            IsFailedWihtException = true;
            throw;
        }
    }

    protected virtual async Task OnCompletedAsync()
    {
        foreach (var handler in CompletedHandlers)
        {
            await handler.Invoke();
        }
    }

    protected virtual async Task OnFailedAsync()
    {
        foreach (var handler in FailedHandlers)
        {
            await handler.Invoke(exception);
        }
    }

    protected virtual async Task OnDisposedAsync()
    {
        foreach (var handler in DisposedHandlers)
        {
            await handler.Invoke();
        }
    }

    public override string ToString()
    {
        return $"[UnitOfWork {UnitWorkId}]";
    }
}