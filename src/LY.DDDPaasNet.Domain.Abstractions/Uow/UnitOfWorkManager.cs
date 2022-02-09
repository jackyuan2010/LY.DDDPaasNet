using Microsoft.Extensions.DependencyInjection;

namespace LY.DDDPaasNet.Domain.Abstractions.Uow;

public class UnitOfWorkManager : IUnitOfWorkManager
{
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly IUnitOfWorkAccessor unitOfWorkAccessor;

    public UnitOfWorkManager(IUnitOfWorkAccessor unitOfWorkAccessor,
        IServiceScopeFactory serviceScopeFactory)
    {
        this.unitOfWorkAccessor = unitOfWorkAccessor;
        this.serviceScopeFactory = serviceScopeFactory;
    }

    public IUnitOfWork Current => unitOfWorkAccessor.UnitOfWork;

    public IUnitOfWork Begin(IUnitOfWorkOptions options, bool requiresNew = false)
    {
        var currentUow = Current;
        if (currentUow != null && !requiresNew)
        {
            return currentUow;
        }

        var unitOfWork = CreateNewUnitWork(options);
        unitOfWork.Initialize(options);

        return unitOfWork;
    }

    private IUnitOfWork CreateNewUnitWork(IUnitOfWorkOptions options)
    {
        var scope = serviceScopeFactory.CreateScope();
        try
        {
            var originalUnitWork = unitOfWorkAccessor.UnitOfWork;

            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            unitOfWorkAccessor.SetUnitOfWork(unitOfWork);

            unitOfWork.OnDisposed(() =>
            {
                //await Task.Run(() => 
                //{
                //    unitWorkAccessor.SetUnitWork(originalUnitWork);
                //    scope.Dispose();
                //});

                unitOfWorkAccessor.SetUnitOfWork(originalUnitWork);
                scope.Dispose();
                return Task.CompletedTask;
            });

            return unitOfWork;
        }
        catch
        {
            scope.Dispose();
            throw;
        }
    }
}