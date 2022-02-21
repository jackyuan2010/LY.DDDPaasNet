using System.Collections.Concurrent;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace LY.DDDPaasNet.Kafka;

public class KafkaMessageConsumer : IKafkaMessageConsumer, IDisposable
{
    private CancellationTokenSource cts = new();

    public ILogger<KafkaMessageConsumer> Logger { get; set; }

    protected IConsumerPool ConsumerPool { get; }

    protected KafkaOptions Options { get; }

    protected Timer Timer { get; }

    protected ConcurrentBag<Func<Message<string, byte[]>, Task>> Callbacks { get; }

    protected IConsumer<string, byte[]> Consumer { get; private set; }

    protected string ConnectionName { get; private set; }

    protected string GroupId { get; private set; }

    protected string TopicName { get; private set; }

    public KafkaMessageConsumer(IOptions<KafkaOptions> options, IConsumerPool consumerPool)
    {
        ConsumerPool = consumerPool;
        Options = options.Value;
        Logger = NullLogger<KafkaMessageConsumer>.Instance;
        Callbacks = new ConcurrentBag<Func<Message<string, byte[]>, Task>>();
    }

    public virtual async void Initialize(string topicName, string groupId, string connectionName = null)
    {
        TopicName = topicName;
        ConnectionName = connectionName ?? KafkaConnections.DefaultConnectionName;
        GroupId = groupId;
        cts = new CancellationTokenSource();
        await Start(cts.Token);
    }

    public virtual void OnMessageReceived(Func<Message<string, byte[]>, Task> callback)
    {
        Callbacks.Add(callback);
    }

    protected virtual async Task Start(CancellationToken cancellationToken)
    {
        await CreateTopicAsync();
        Consume(cancellationToken);
    }

    protected virtual async Task CreateTopicAsync()
    {
        using (var adminClient = new AdminClientBuilder(Options.Connections.GetOrDefault(ConnectionName)).Build())
        {
            var topic = new TopicSpecification
            {
                Name = TopicName,
                NumPartitions = 1,
                ReplicationFactor = 1
            };

            Options.ConfigureTopic?.Invoke(topic);

            try
            {
                await adminClient.CreateTopicsAsync(new[] { topic });
            }
            catch (CreateTopicsException e)
            {
                if (e.Results.Any(x => x.Error.Code != ErrorCode.TopicAlreadyExists))
                {
                    throw;
                }
            }
        }
    }

    protected virtual void Consume(CancellationToken cancellationToken)
    {
        Consumer = ConsumerPool.Get(GroupId, ConnectionName);

        Task.Factory.StartNew(async () =>
        {
            Consumer.Subscribe(TopicName);

            while (true)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var consumeResult = Consumer.Consume();

                    if (consumeResult.IsPartitionEOF)
                    {
                        continue;
                    }

                    await HandleIncomingMessage(consumeResult, cancellationToken);
                }
                catch (ConsumeException ex)
                {
                    Logger.LogError(ex, ex.Message);
                }
            }
        }, TaskCreationOptions.LongRunning);
    }

    protected virtual async Task HandleIncomingMessage(ConsumeResult<string, byte[]> consumeResult, CancellationToken cancellationToken)
    {
        try
        {
            foreach (var callback in Callbacks)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await callback(consumeResult.Message);
            }
        }
        catch(OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
        }
        finally
        {
            Consumer.Commit(consumeResult);
        }
    }

    protected virtual void Stop()
    {
        cts.Cancel();
    }

    public virtual void Dispose()
    {
        if (Consumer == null)
        {
            return;
        }

        Stop();
        cts.Dispose();

        Consumer.Close();
        Consumer.Dispose();
    }
}