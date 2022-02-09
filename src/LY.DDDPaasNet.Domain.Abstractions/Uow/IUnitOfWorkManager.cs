namespace LY.DDDPaasNet.Domain.Abstractions.Uow;

public interface IUnitOfWorkManager
{
    IUnitOfWork Current { get; }

    IUnitOfWork Begin(IUnitOfWorkOptions options, bool requiresNew = false);
}