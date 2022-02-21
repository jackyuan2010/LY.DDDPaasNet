namespace LY.DDDPaasNet.Kafka;

public interface IProducerPool : IDisposable
{
    IProducer<string, byte[]> Get(string connectionName = null);
}