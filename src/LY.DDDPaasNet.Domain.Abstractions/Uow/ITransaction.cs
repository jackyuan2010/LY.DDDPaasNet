namespace LY.DDDPaasNet.Domain.Abstractions.Uow;

public interface ITransaction : IDisposable
{
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}