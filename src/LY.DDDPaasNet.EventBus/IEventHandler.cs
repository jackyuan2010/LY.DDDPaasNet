namespace LY.DDDPaasNet.EventBus;

public interface IEventHandler
{
}

public interface IEventHandler<in TEvent> : IEventHandler
{
    Task HandleEventAsync(TEvent eventData);
}