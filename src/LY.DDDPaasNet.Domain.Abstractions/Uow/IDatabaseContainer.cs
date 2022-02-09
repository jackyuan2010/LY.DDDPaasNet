namespace LY.DDDPaasNet.Domain.Abstractions.Uow;

public interface IDatabaseContainer
{
    IDatabase GetDatabase(string key);

    void AddDatabase(string key, IDatabase database);

    IDatabase GetOrAddDatabase(string key, Func<IDatabase> factory);
}