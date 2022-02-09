namespace LY.DDDPaasNet.Domain.Abstractions.Uow;

public class AsyncLocalUnitOfWorkAccessor : IUnitOfWorkAccessor
{
    private readonly AsyncLocal<IUnitOfWork> currentUow;

    public IUnitOfWork UnitOfWork => currentUow.Value;

    public void SetUnitOfWork(IUnitOfWork unitOfWork)
    {
        currentUow.Value = unitOfWork;
    }

    public AsyncLocalUnitOfWorkAccessor()
    {
        currentUow = new AsyncLocal<IUnitOfWork>();
    }
}