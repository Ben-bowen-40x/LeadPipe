namespace LeadPipe.Core;

public static class EnumerableExtensions
{
    public static Dictionary<TKey, TValue> ToDictionaryFast<TSource, TKey, TValue>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> key,
        Func<TSource, TValue> value
    ) where TKey : notnull
    {
        if (source is ICollection<TSource> collection)
            return collection.ToDictionaryFast(key, value);

        int capacity = source.GetCapacityOrDefault();

        var result = new Dictionary<TKey, TValue>(capacity);

        foreach (var item in source)
            result.TryAdd(key(item), value(item));

        return result;
    }

    public static Dictionary<TKey, TSource> ToDictionaryFast<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> key
    ) where TKey : notnull
    {
        if (source is ICollection<TSource> collection)
            return collection.ToDictionaryFast(key);

        int capacity = source.GetCapacityOrDefault();

        var result = new Dictionary<TKey, TSource>(capacity);

        foreach (var item in source)
            result.TryAdd(key(item), item);

        return result;
    }

    private static int GetCapacityOrDefault<T>(this IEnumerable<T> source) => source.TryGetNonEnumeratedCount(out var c) ? c : 0;
}