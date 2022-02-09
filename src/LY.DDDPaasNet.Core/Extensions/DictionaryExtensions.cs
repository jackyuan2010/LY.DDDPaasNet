namespace LY.DDDPaasNet.Core.Extensions;

public static class DictionaryExtensions
{
    internal static bool TryGetValue<T>(this IDictionary<string, object> dictionary, string key, out T value)
    {
        object valueObj;
        if (dictionary.TryGetValue(key, out valueObj) && valueObj is T)
        {
            value = (T)valueObj;
            return true;
        }

        value = default;
        return false;
    }

    public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
    {
        TValue obj;
        return dictionary.TryGetValue(key, out obj) ? obj : default;
    }

    public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
    {
        return dictionary.TryGetValue(key, out var obj) ? obj : default;
    }

    public static TValue GetOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key)
    {
        return dictionary.TryGetValue(key, out var obj) ? obj : default;
    }

    public static TValue GetOrDefault<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key)
    {
        return dictionary.TryGetValue(key, out var obj) ? obj : default;
    }

    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> factory)
    {
        TValue obj;
        if (dictionary.TryGetValue(key, out obj))
        {
            return obj;
        }

        return dictionary[key] = factory(key);
    }

    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> factory)
    {
        return dictionary.GetOrAdd(key, k => factory());
    }

    public static TValue GetOrAdd<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> factory)
    {
        return dictionary.GetOrAdd(key, k => factory());
    }
}