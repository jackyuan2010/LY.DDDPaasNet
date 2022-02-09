namespace LY.DDDPaasNet.Domain.Abstractions.Uow;

public interface ITransactionContainer
{
    ITransaction GetTransaction(string key);

    void AddTransaction(string key, ITransaction transaction);

    ITransaction GetOrAddTransaction(string key, Func<ITransaction> factory);
}