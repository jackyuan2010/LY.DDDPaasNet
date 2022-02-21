﻿using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace LY.DDDPaasNet.Kafka;

public class ProducerPool : IProducerPool
{
    protected KafkaOptions Options { get; }

    protected ConcurrentDictionary<string, Lazy<IProducer<string, byte[]>>> Producers { get; }

    protected TimeSpan TotalDisposeWaitDuration { get; set; } = TimeSpan.FromSeconds(10);

    public ILogger<ProducerPool> Logger { get; set; }

    private bool isDisposed;

    public ProducerPool(IOptions<KafkaOptions> options)
    {
        Options = options.Value;

        Producers = new ConcurrentDictionary<string, Lazy<IProducer<string, byte[]>>>();
        Logger = new NullLogger<ProducerPool>();
    }

    public virtual IProducer<string, byte[]> Get(string connectionName = null)
    {
        connectionName ??= KafkaConnections.DefaultConnectionName;

        return Producers.GetOrAdd(
            connectionName, connection => new Lazy<IProducer<string, byte[]>>(() =>
            {
                var config = Options.Connections.GetOrDefault(connection);

                Options.ConfigureProducer?.Invoke(new ProducerConfig(config));

                return new ProducerBuilder<string, byte[]>(config).Build();
            })).Value;
    }

    public void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        isDisposed = true;

        if (!Producers.Any())
        {
            Logger.LogDebug($"Disposed producer pool with no producers in the pool.");
            return;
        }

        var poolDisposeStopwatch = Stopwatch.StartNew();

        Logger.LogInformation($"Disposing producer pool ({Producers.Count} producers).");

        var remainingWaitDuration = TotalDisposeWaitDuration;

        foreach (var producer in Producers.Values)
        {
            var poolItemDisposeStopwatch = Stopwatch.StartNew();

            try
            {
                producer.Value.Dispose();
            }
            catch
            {
            }

            poolItemDisposeStopwatch.Stop();

            remainingWaitDuration = remainingWaitDuration > poolItemDisposeStopwatch.Elapsed
                ? remainingWaitDuration.Subtract(poolItemDisposeStopwatch.Elapsed)
                : TimeSpan.Zero;
        }

        poolDisposeStopwatch.Stop();

        Logger.LogInformation(
            $"Disposed Kafka Producer Pool ({Producers.Count} producers in {poolDisposeStopwatch.Elapsed.TotalMilliseconds:0.00} ms).");

        if (poolDisposeStopwatch.Elapsed.TotalSeconds > 5.0)
        {
            Logger.LogWarning(
                $"Disposing Kafka Producer Pool got time greather than expected: {poolDisposeStopwatch.Elapsed.TotalMilliseconds:0.00} ms.");
        }

        Producers.Clear();
    }
}