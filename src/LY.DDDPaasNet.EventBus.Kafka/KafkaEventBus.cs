using Confluent.Kafka;
using LY.DDDPaasNet.Core.Extensions;
using LY.DDDPaasNet.Core.Serializer;
using LY.DDDPaasNet.Domain.Abstractions.Uow;
using LY.DDDPaasNet.Kafka;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace LY.DDDPaasNet.EventBus.Kafka;

public class KafkaEventBus : EventBusBase
{
    protected KafkaEventBusOptions KafkaEventBusOptions { get; }
    protected IKafkaMessageConsumerFactory MessageConsumerFactory { get; }
    protected IProducerPool ProducerPool { get; }
    protected ConcurrentDictionary<string, Type> EventTypes { get; }
    protected IKafkaMessageConsumer Consumer { get; private set; }
    protected ConcurrentDictionary<Type, List<IEventHandler>> EventHandlers { get; }

    public KafkaEventBus(
        IUnitOfWorkManager unitOfWorkManager,
        IOptions<KafkaEventBusOptions> kafkaEventBusOptions,
        IKafkaMessageConsumerFactory messageConsumerFactory,
        IProducerPool producerPool)
        : base(unitOfWorkManager)
    {
        KafkaEventBusOptions = kafkaEventBusOptions.Value;
        MessageConsumerFactory = messageConsumerFactory;
        ProducerPool = producerPool;
        EventTypes = new ConcurrentDictionary<string, Type>();
        EventHandlers = new ConcurrentDictionary<Type, List<IEventHandler>>();
    }

    public void Initialize()
    {
        Consumer = MessageConsumerFactory.Create(
            KafkaEventBusOptions.TopicName,
            KafkaEventBusOptions.GroupId,
            KafkaEventBusOptions.ConnectionName);
        Consumer.OnMessageReceived(ProcessEventAsync);
    }

    private async Task ProcessEventAsync(Message<string, byte[]> message)
    {
        var eventName = message.Key;
        var eventType = EventTypes.GetOrDefault(eventName);
        if (eventType == null)
        {
            return;
        }

        var eventHandlers = GetOrCreateEventHandlers(eventType);
        foreach(IEventHandler handler in eventHandlers)
        {
        }
    }

    public override void Subscribe(Type eventType, IEventHandler handler)
    {
        lock (EventHandlers)
        {
            var eventHandlers = GetOrCreateEventHandlers(eventType);

            if (!eventHandlers.Contains(handler))
            {
                eventHandlers.Add(handler);
            }
        }
    }

    public override void Unsubscribe<TEvent>(Func<TEvent, Task> action)
    {
        lock(EventHandlers)
        {
            var eventHandlers = GetOrCreateEventHandlers(typeof(TEvent));
            eventHandlers.RemoveAll(x => x is ActionEventHandler<TEvent> 
                && ((ActionEventHandler<TEvent>)x).Action == action);
        }
    }

    public override void Unsubscribe(Type eventType, IEventHandler handler)
    {
        lock (EventHandlers)
        {
            var eventHandlers = GetOrCreateEventHandlers(eventType);
            eventHandlers.RemoveAll(x => x == handler);
        }
    }

    public override void UnsubscribeAll(Type eventType)
    {
        lock (EventHandlers)
        {
            var eventHandlers = GetOrCreateEventHandlers(eventType);
            eventHandlers.Clear();
        }
    }

    private List<IEventHandler> GetOrCreateEventHandlers(Type eventType)
    {
        return EventHandlers.GetOrAdd(
            eventType,
            type =>
            {
                var eventName = eventType.Name;
                EventTypes[eventName] = type;
                return new List<IEventHandler>();
            }
        );
    }

    public override async Task PublishAsync(Type eventType, object eventData, bool onUnitOfWorkComplete = true)
    {
        if (onUnitOfWorkComplete && UnitOfWorkManager.Current != null)
        {
            return;
        }

        await PublishAsync(eventType, eventData);
    }

    public virtual async Task PublishAsync(Type eventType, object eventData, Headers headers, Dictionary<string, object> headersArguments)
    {
        await PublishAsync(
            KafkaEventBusOptions.TopicName,
            eventType,
            eventData,
            headers,
            headersArguments
        );
    }

    private Task PublishAsync(string topicName, Type eventType, object eventData, Headers headers, Dictionary<string, object> headersArguments)
    {
        var eventName = eventType.Name;
        var body = DefaultObjectSerializer.Serialize(eventData);

        return PublishAsync(topicName, eventName, body, headers, headersArguments);
    }

    protected async Task PublishAsync(Type eventType, object eventData)
    {
        await PublishAsync(
            eventType,
            eventData,
            new Headers
            {
                { "messageId", System.Text.Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N")) }
            },
            null
        );
    }

    private async Task PublishAsync(string topicName, string eventName, byte[] body, Headers headers, Dictionary<string, object> headersArguments)
    {
        var producer = ProducerPool.Get(KafkaEventBusOptions.ConnectionName);

        SetEventMessageHeaders(headers, headersArguments);

        await producer.ProduceAsync(
            topicName,
            new Message<string, byte[]>
            {
                Key = eventName,
                Value = body,
                Headers = headers
            });
    }

    private void SetEventMessageHeaders(Headers headers, Dictionary<string, object> headersArguments)
    {
        if (headersArguments == null)
        {
            return;
        }

        foreach (var header in headersArguments)
        {
            headers.Remove(header.Key);
            headers.Add(header.Key, DefaultObjectSerializer.Serialize(header.Value));
        }
    }
}