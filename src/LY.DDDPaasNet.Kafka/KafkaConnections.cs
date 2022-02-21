namespace LY.DDDPaasNet.Kafka;

[Serializable]
public class KafkaConnections : Dictionary<string, ClientConfig>
{
    public const string DefaultConnectionName = "Default";

    public ClientConfig Default {
        get => this[DefaultConnectionName];
        set => this[DefaultConnectionName] = value;
    }

    public KafkaConnections()
    {
        Default = new ClientConfig();
    }

    public ClientConfig GetOrDefault(string connectionName)
    {
        if (TryGetValue(connectionName, out var connectionFactory))
        {
            return connectionFactory;
        }

        return Default;
    }
}