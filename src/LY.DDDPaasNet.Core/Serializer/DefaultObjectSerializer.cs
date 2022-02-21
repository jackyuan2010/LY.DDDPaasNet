using System.Text.Json;

namespace LY.DDDPaasNet.Core.Serializer;

public static class DefaultObjectSerializer
{
    public static byte[] Serialize<T>(T obj)
    {
        if (obj == null)
        {
            return null;
        }

        return JsonSerializer.SerializeToUtf8Bytes(obj);
    }

    public static T Deserialize<T>(byte[] bytes)
    {
        if (bytes == null)
        {
            return default;
        }
        return JsonSerializer.Deserialize<T>(bytes);
    }
}