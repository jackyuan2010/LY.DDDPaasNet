namespace LY.DDDPaasNet.Kafka;

public interface IConsumerPool : IDisposable
{
    IConsumer<string, byte[]> Get(string groupId, string connectionName = null);
}