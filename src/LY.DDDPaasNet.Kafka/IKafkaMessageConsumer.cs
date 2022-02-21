namespace LY.DDDPaasNet.Kafka;

public interface IKafkaMessageConsumer
{
    void OnMessageReceived(Func<Message<string, byte[]>, Task> callback);
}