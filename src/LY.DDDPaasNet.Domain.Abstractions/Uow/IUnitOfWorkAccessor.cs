namespace LY.DDDPaasNet.Domain.Abstractions.Uow;

public interface IUnitOfWorkAccessor
{
    IUnitOfWork UnitOfWork { get; }

    void SetUnitOfWork(IUnitOfWork unitOfWork);
}