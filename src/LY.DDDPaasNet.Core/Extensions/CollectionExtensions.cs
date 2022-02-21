namespace System.Collections.Generic;

public static class CollectionExtensions
{
    public static bool IsNullOrEmpty<T>(this ICollection<T> source)
    {
        return source == null || !source.Any();
    }

    public static bool IsNotNullOrEmpty<T>(this ICollection<T> source)
    {
        return source != null && source.Any();
    }

    //public static bool IsNullOrEmpty<T>(this IReadOnlyCollection<T> source)
    //{
    //    return source == null || !source.Any();
    //}

    //public static bool IsNotNullOrEmpty<T>(this IReadOnlyCollection<T> source)
    //{
    //    return source != null && source.Any();
    //}

    public static bool AddIfNotContains<T>(this ICollection<T> source, T item)
    {
        if (source.Contains(item))
        {
            return false;
        }

        source.Add(item);
        return true;
    }

    public static IEnumerable<T> AddIfNotContains<T>(this ICollection<T> source, IEnumerable<T> items)
    {
        var addedItems = new List<T>();

        foreach (var item in items)
        {
            if (source.Contains(item))
            {
                continue;
            }

            source.Add(item);
            addedItems.Add(item);
        }

        return addedItems;
    }

    public static bool AddIfNotContains<T>(this ICollection<T> source, Func<T, bool> predicate, Func<T> itemFactory)
    {
        if (source.Any(predicate))
        {
            return false;
        }

        source.Add(itemFactory());
        return true;
    }

    public static IList<T> RemoveAll<T>(this ICollection<T> source, Func<T, bool> predicate)
    {
        var items = source.Where(predicate).ToList();

        foreach (var item in items)
        {
            source.Remove(item);
        }

        return items;
    }

    public static void RemoveAll<T>(this ICollection<T> source, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            source.Remove(item);
        }
    }
}