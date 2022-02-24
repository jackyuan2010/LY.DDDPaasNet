using System.Text;
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

    public static byte[] Serialize(object obj)
    {
        return JsonSerializer.SerializeToUtf8Bytes(obj);
    }

    public static string SerializeToString<T>(T obj)
    {
        if (obj == null)
        {
            return null;
        }

        return JsonSerializer.Serialize(obj);
    }

    public static string SerializeToString(object obj)
    {
        return JsonSerializer.Serialize(obj);
    }

    public static T Deserialize<T>(byte[] bytes)
    {
        if (bytes == null)
        {
            return default;
        }
        return JsonSerializer.Deserialize<T>(bytes);
    }

    public static object Deserialize(Type type, byte[] value)
    {
        return Deserialize(type, Encoding.UTF8.GetString(value));
    }

    public static object Deserialize(Type type, string jsonString, bool camelCase = true)
    {
        return JsonSerializer.Deserialize(jsonString, type, CreateJsonSerializerOptions(camelCase));
    }

    private static JsonSerializerOptions CreateJsonSerializerOptions(bool camelCase = true, bool indented = false)
    {
        var settings = new JsonSerializerOptions();

        if (camelCase)
        {
            settings.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        }

        if (indented)
        {
            settings.WriteIndented = true;
        }

        return settings;
    }
}