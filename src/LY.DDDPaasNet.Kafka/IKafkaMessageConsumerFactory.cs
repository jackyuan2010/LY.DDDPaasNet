namespace LY.DDDPaasNet.Kafka;

public interface IKafkaMessageConsumerFactory
{
    IKafkaMessageConsumer Create(
        string topicName,
        string groupId,
        string connectionName = null);
}