using LY.DDDPaasNet.Core.DependencyInjection;

namespace LY.DDDPaasNet.Domain.Abstractions.Uow;

public interface IUnitOfWork : IDatabaseContainer, ITransactionContainer, IServiceProviderAccessor, IDisposable
{
    Guid UnitWorkId { get; }

    bool IsDisposed { get; }

    bool IsCompleted { get; }

    bool IsFailedWihtException { get; }

    IUnitOfWorkOptions Options { get; }

    void Initialize(IUnitOfWorkOptions options);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    Task CompleteAsync(CancellationToken cancellationToken = default);

    Task RollbackAsync(CancellationToken cancellationToken = default);

    void OnCompleted(Func<Task> handler);

    void OnFailed(Func<Exception, Task> handler);

    void OnDisposed(Func<Task> handler);
}