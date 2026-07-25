using System.Linq;

namespace Content.Shared._DVA.Utility;

public static class ThresholdHelpers
{
    public static TValue? HighestMatch<TKey, TValue>(this SortedDictionary<TKey, TValue> dictionary, TKey value) where TKey : IComparable<TKey> where TValue : struct
    {
        foreach (var (threshold, data) in dictionary.Reverse())
        {
            if (value.CompareTo(threshold) < 0)
                continue;

            return data;
        }

        return null;
    }

    public static TValue? HighestMatchClass<TKey, TValue>(this SortedDictionary<TKey, TValue> dictionary, TKey value) where TKey : IComparable<TKey> where TValue : class
    {
        foreach (var (threshold, data) in dictionary.Reverse())
        {
            if (value.CompareTo(threshold) < 0)
                continue;

            return data;
        }

        return null;
    }

    public static TValue? LowestMatch<TKey, TValue>(this SortedDictionary<TKey, TValue> dictionary, TKey value) where TKey : IComparable<TKey> where TValue : struct
    {
        foreach (var (threshold, data) in dictionary)
        {
            if (value.CompareTo(threshold) > 0)
                continue;

            return data;
        }

        return null;
    }

    public static TValue? LowestMatchClass<TKey, TValue>(this SortedDictionary<TKey, TValue> dictionary, TKey value) where TKey : IComparable<TKey> where TValue : class
    {
        foreach (var (threshold, data) in dictionary)
        {
            if (value.CompareTo(threshold) > 0)
                continue;

            return data;
        }

        return null;
    }
}
