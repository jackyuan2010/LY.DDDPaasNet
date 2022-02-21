using LY.DDDPaasNet.Domain.Abstractions.Uow;

namespace LY.DDDPaasNet.EventBus;
public abstract class EventBusBase : IEventBus
{
    protected IUnitOfWorkManager UnitOfWorkManager { get; }

    protected EventBusBase(IUnitOfWorkManager unitOfWorkManager)
    {
        UnitOfWorkManager = unitOfWorkManager;
    }

    public Task PublishAsync<TEvent>(TEvent eventData, bool onUnitOfWorkComplete = true)
    where TEvent : class
    {
        return PublishAsync(typeof(TEvent), eventData, onUnitOfWorkComplete);
    }

    public abstract Task PublishAsync(Type eventType, object eventData, bool onUnitOfWorkComplete = true);

    public void Subscribe<TEvent>(IEventHandler<TEvent> handler) where TEvent : class
    {
        Subscribe(typeof(TEvent), handler);
    }

    public abstract void Subscribe(Type eventType, IEventHandler handler);

    public virtual void Subscribe<TEvent>(Func<TEvent, Task> action) where TEvent : class
    {
        Subscribe(typeof(TEvent), new ActionEventHandler<TEvent>(action));
    }

    public virtual void Unsubscribe<TEvent>(IEventHandler<TEvent> handler) where TEvent : class
    {
        Unsubscribe(typeof(TEvent), handler);
    }

    public abstract void Unsubscribe(Type eventType, IEventHandler handler);

    public abstract void Unsubscribe<TEvent>(Func<TEvent, Task> action) where TEvent : class;

    public virtual void UnsubscribeAll<TEvent>() where TEvent : class
    {
        UnsubscribeAll(typeof(TEvent));
    }

    public abstract void UnsubscribeAll(Type eventType);
}