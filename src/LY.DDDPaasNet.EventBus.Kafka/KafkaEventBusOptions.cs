namespace LY.DDDPaasNet.EventBus.Kafka;

public class KafkaEventBusOptions
{

    public string ConnectionName { get; set; }

    public string TopicName { get; set; }

    public string GroupId { get; set; }
}