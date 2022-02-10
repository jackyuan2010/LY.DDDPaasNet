namespace LY.DDDPaasNet.Domain.Abstractions.Uow;

public interface IDatabase
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}