namespace LY.DDDPaasNet.EventBus;

public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent eventData, bool onUnitOfWorkComplete = true)
        where TEvent : class;

    Task PublishAsync(Type eventType, object eventData, bool onUnitOfWorkComplete = true);

    void Subscribe<TEvent>(IEventHandler<TEvent> handler)
        where TEvent : class;

    void Subscribe(Type eventType, IEventHandler handler);

    void Subscribe<TEvent>(Func<TEvent, Task> action)
        where TEvent : class;

    void Unsubscribe<TEvent>(IEventHandler<TEvent> handler)
        where TEvent : class;

    void Unsubscribe(Type eventType, IEventHandler handler);

    void Unsubscribe<TEvent>(Func<TEvent, Task> action)
        where TEvent : class;

    void UnsubscribeAll<TEvent>()
        where TEvent : class;

    void UnsubscribeAll(Type eventType);
}