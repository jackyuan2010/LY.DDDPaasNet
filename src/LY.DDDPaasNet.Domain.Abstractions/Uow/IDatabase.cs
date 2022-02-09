namespace LY.DDDPaasNet.Domain.Abstractions.Uow;

public interface IDatabase
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}