using Ampere.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

#nullable enable

namespace Ampere.Enumerable;

/// <summary>
/// A static utility class for .NET <see cref="IEnumerable{T}"/>
/// </summary>
public static class EnumerableUtils
{
    private static readonly System.Globalization.CultureInfo CulInv = System.Globalization.CultureInfo.InvariantCulture;

    /// <summary>
    /// Concatenates all IEnumerables which are specified in the parameter. The
    /// concatenation occurs in the order specified in the parameter.
    /// </summary>
    /// <typeparam name="T">The enumerable type to be used</typeparam>
    /// <param name="ie">An enumerable of all one dimensional arrays to be concatenated</param>
    /// <exception cref="ArgumentNullException">Is thrown if any enumerable, which is a candidate to be concatenated, is null</exception>
    /// <returns>A single enumerable with all the concatenated elements</returns>
    public static IEnumerable<T> Fuse<T>(params IEnumerable<T>[] ie)
    {
        ArgumentNullException.ThrowIfNull(ie);

        foreach (var x in ie)
        {
            if (x is null)
                throw new ArgumentNullException(nameof(ie), "One of the params array's were null");
        }

        var totalLength = ie.Sum(t => t is ICollection<T> col ? col.Count : t.Count());
        var result = new T[totalLength];

        var dest = 0;
        foreach (var t in ie)
        {
            var arr = t as T[] ?? t.ToArray();
            Array.Copy(arr, 0, result, dest, arr.Length);
            dest += arr.Length;
        }
        return result;
    }

    /// <summary>
    /// Extends a generic array buffer by a specified size given a predefined condition
    /// is satisfied.
    /// </summary>
    /// <typeparam name="T">The type of the array</typeparam>
    /// <param name="buf">The buffer to be used</param>
    /// <param name="condition">The condition that should be satisfied before Array resizing</param>
    /// <param name="size">The size in which to resize the buffer if the condition is satisfied</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CheckBufferAndExtend<T>(ref T[] buf, bool condition, int size)
    {
        if (condition)
            Array.Resize(ref buf, buf.Length + size);
    }

    /// <summary>
    /// Extends an <see cref="IEnumerable{T}"/> buffer by a specified size given a predefined condition
    /// is satisfied.
    /// </summary>
    /// <typeparam name="T">The type of the IEnumerable</typeparam>
    /// <param name="buf">The buffer to be used</param>
    /// <param name="condition">The condition that should be satisfied before Array resizing</param>
    /// <param name="size">The size in which to resize the buffer if the condition is satisfied</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CheckBufferAndExtend<T>(ref IEnumerable<T> buf, bool condition, int size)
    {
        ArgumentNullException.ThrowIfNull(buf);
        if (!condition) return;
        var x = buf as T[] ?? buf.ToArray();
        Array.Resize(ref x, x.Length + size);
        buf = x;
    }

    /// <summary>
    /// Inserts the specified element at the specified index in the generic array (modifying the original array).
    /// If element at that position exists, it shifts that element and any subsequent elements to the right,
    /// adding one to their indices. The method also allows for inserting more than one element into
    /// the array at one time given that they are specified.
    /// </summary>
    /// <typeparam name="T">The type of the array</typeparam>
    /// <param name="src">The generic array to be used</param>
    /// <param name="startIdx">The index to start insertion</param>
    /// <param name="amtToIns">The amount of elements to insert into the array</param>
    /// <param name="valuesToIns">Optionally, the values to insert into the empty indices of the new array</param>
    /// <exception cref="ArgumentNullException">Thrown when src or valuesToIns is null</exception>
    /// <exception cref="IndexOutOfRangeException">Thrown when the valuesToIns array does not match the amount to insert (if it is greater than 0)</exception>
    /// <exception cref="IndexOutOfRangeException">Thrown when the amtToIns or the startIdx is less than 0</exception>
    public static void Insert<T>(ref T[] src, int startIdx, int amtToIns, params T[] valuesToIns)
    {
        ArgumentNullException.ThrowIfNull(src);
        ArgumentNullException.ThrowIfNull(valuesToIns);

        var len = src.Length;

        if (startIdx < 0 || startIdx >= len || amtToIns < 0)
            throw new IndexOutOfRangeException();

        if (amtToIns != valuesToIns.Length && valuesToIns.Length != 0)
            throw new IndexOutOfRangeException("offset amount should equal the number of values to be filled");

        var arrManaged = new T[len + amtToIns];

        if (amtToIns != 0)
        {
            if (startIdx == len - 1)
            {
                Array.ConstrainedCopy(src, 0, arrManaged, 0, len);
                Array.ConstrainedCopy(valuesToIns, 0, arrManaged, startIdx + 1, valuesToIns.Length);
            }
            else
            {
                Array.ConstrainedCopy(src, 0, arrManaged, 0, startIdx);
                Array.ConstrainedCopy(valuesToIns, 0, arrManaged, startIdx, valuesToIns.Length);
                Array.ConstrainedCopy(src, startIdx, arrManaged, startIdx + amtToIns, len - startIdx);
            }
        }
        src = arrManaged;
    }

    /// <summary>
    /// Inserts the specified element at the specified index in the enumerable (modifying the original enumerable).
    /// If element at that position exists, it shifts that element and any subsequent elements to the right.
    /// </summary>
    /// <typeparam name="T">The type of the enumerable</typeparam>
    /// <param name="src">The IEnumerable to be used</param>
    /// <param name="startIdx">The index to start insertion</param>
    /// <param name="amtToIns">The amount of elements to insert into the enumerable</param>
    /// <param name="valuesToIns">Optionally, the values to insert into the empty indices of the new enumerable</param>
    public static void Insert<T>(ref IEnumerable<T> src, int startIdx, int amtToIns, params T[] valuesToIns)
    {
        ArgumentNullException.ThrowIfNull(src);
        var enumerable = src as T[] ?? src.ToArray();
        Insert(ref enumerable, startIdx, amtToIns, valuesToIns);
        src = enumerable;
    }

    /// <summary>
    /// Given a primary enumerable to check against and one or more test enumerables, InnerContains executes an intersection
    /// of the primary and test enumerables to verify if all values in the test are contained within the primary enumerable.
    /// </summary>
    /// <typeparam name="T">The type of the IEnumerable</typeparam>
    /// <param name="ie">The primary IEnumerable to check against</param>
    /// <param name="isAll">Indicates whether to check whether all values are intersected or partially intersected</param>
    /// <param name="otherArrays">An array of Enumerables to test against the primary enumerable</param>
    /// <returns>An instance of the <see cref="InnerContainsProgram{T}"/> to verify the result and violating enumerables</returns>
    public static InnerContainsProgram<T> InnerContains<T>(this IEnumerable<T> ie, bool isAll, params IEnumerable<T>[] otherArrays)
    {
        ArgumentNullException.ThrowIfNull(ie);
        ArgumentNullException.ThrowIfNull(otherArrays);
        return new InnerContainsProgram<T>(ie, isAll, otherArrays);
    }

    /// <summary>
    /// Returns whether an IEnumerable is null or empty.
    /// </summary>
    /// <typeparam name="T">The type of the IEnumerable</typeparam>
    /// <param name="ie">The IEnumerable to be used</param>
    /// <returns>True if null or empty</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? ie) => ie is null || !ie.Any();

    /// <summary>
    /// Returns true if the Count of the IEnumerable is zero or one.
    /// </summary>
    /// <typeparam name="T">The type of the IEnumerable</typeparam>
    /// <param name="ie">The IEnumerable to be used</param>
    /// <returns>True if the Count of the IEnumerable is zero or one</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsZeroOrOne<T>(this IEnumerable<T> ie)
    {
        ArgumentNullException.ThrowIfNull(ie);
        using var enumerator = ie.GetEnumerator();
        return !enumerator.MoveNext() || !enumerator.MoveNext();
    }

    /// <summary>
    /// Enables python style for-loop for easier readability. This loop begins
    /// at the starting value and loops until the end - 1.
    /// </summary>
    /// <param name="start">The starting counter for the loop (inclusive)</param>
    /// <param name="end">The ending counter for the loop (exclusive)</param>
    /// <returns>An IEnumerable representing the current index</returns>
    public static IEnumerable<int> Range(int start, int end)
    {
        for (var i = start; i < end; i++)
            yield return i;
    }

    /// <summary>
    /// Cryptographically shuffles an enumerable.
    /// </summary>
    /// <typeparam name="T">The element type of the IEnumerable</typeparam>
    /// <param name="src">The IEnumerable</param>
    /// <returns>The Shuffled IEnumerable</returns>
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> src)
    {
        ArgumentNullException.ThrowIfNull(src);
        return Shuffler.ShuffleCopy(src);
    }

    /// <summary>
    /// Enables python style for-loop for easier readability. This loop begins
    /// at the starting value and loops until the end (inclusive).
    /// </summary>
    /// <param name="start">The starting counter for the loop (inclusive)</param>
    /// <param name="end">The ending counter for the loop (inclusive)</param>
    /// <returns>An IEnumerable representing the current index</returns>
    public static IEnumerable<int> Span(int start, int end)
    {
        for (var i = start; i <= end; i++)
            yield return i;
    }

    /// <summary>
    /// Prints a string representation of an enumerable. There are 4 supported lengths for the fmtExp. The
    /// default length is 0 and the default behavior depends on the type of the enumerable. If the type is primitive
    /// (based on the System.IsPrimitive property) including decimal and string, then it prints the enumerable with a space
    /// as a separator between each element. A fmtExp of length 1 specifies a character to separate each element.
    /// A fmtExp of length 2 specifies two characters to mark the left and right outer bounds of the enumerable.
    /// A fmtExp of length 3 specifies a character for the left outer bound, a separator character,
    /// and a character for the right outer bound. The "/0+" expression disables separators.
    /// </summary>
    /// <typeparam name="T">The type of the enumerable</typeparam>
    /// <param name="src">The IEnumerable to be used</param>
    /// <param name="fmtExp">The defined expression to be optionally used</param>
    /// <param name="evenlySpacedSeparator">Determines whether the spacing between each element should be the same</param>
    /// <returns>The string representation of the enumerable</returns>
    /// <exception cref="ArgumentNullException">If arr is null</exception>
    /// <exception cref="FormatException">If the formatting expression length is neither 0 nor 3</exception>
    public static string ToString<T>(this IEnumerable<T> src, string fmtExp = "", bool evenlySpacedSeparator = false)
    {
        ArgumentNullException.ThrowIfNull(fmtExp);
        ArgumentNullException.ThrowIfNull(src);

        var frl = fmtExp.Length;
        if (frl > 3)
            throw new FormatException("Unsupported Expression");

        string outerLeft = string.Empty, separator = string.Empty, outerRight = string.Empty;
        var hasNoSep = false;
        if (fmtExp.Equals("/0+", StringComparison.InvariantCulture))
        {
            hasNoSep = true;
            frl = 1;
        }

        switch (frl)
        {
            case 3:
                outerLeft = fmtExp[0].ToString(CulInv);
                separator = fmtExp[1].ToString(CulInv);
                outerRight = fmtExp[2].ToString(CulInv);
                break;
            case 2:
                outerLeft = fmtExp[0].ToString(CulInv);
                outerRight = fmtExp[1].ToString(CulInv);
                break;
            case 1:
                separator = fmtExp[0].ToString(CulInv);
                break;
        }

        var type = typeof(T);
        var isLooselyPrimitive = type.IsPrimitive || type == typeof(decimal) || type == typeof(string);

        var sb = new StringBuilder();
        sb.Append(outerLeft);

        var enumerable = src as T[] ?? src.ToArray();
        var len = enumerable.Length;

        for (var i = 0; i < len; i++)
        {
            if (i == len - 1)
            {
                sb.Append(enumerable[i]);
            }
            else if (frl == 1 && hasNoSep)
            {
                sb.Append(enumerable[i]);
            }
            else if (evenlySpacedSeparator && isLooselyPrimitive && frl != 2)
            {
                sb.Append(enumerable[i]).Append(" ").Append(separator).Append(' ');
            }
            else if (isLooselyPrimitive)
            {
                sb.Append(enumerable[i]).Append(separator).Append(' ');
            }
            else
            {
                sb.Append(enumerable[i]).Append(separator);
            }
        }
        sb.Append(outerRight);
        return sb.ToString();
    }

    /// <summary>
    /// Splits the source enumerable into batches of the specified size.
    /// The last batch may contain fewer elements than the batch size.
    /// </summary>
    /// <typeparam name="T">The element type</typeparam>
    /// <param name="src">The source enumerable</param>
    /// <param name="batchSize">The maximum number of elements per batch</param>
    /// <returns>An enumerable of batches</returns>
    public static IEnumerable<T[]> Batch<T>(this IEnumerable<T> src, int batchSize)
    {
        ArgumentNullException.ThrowIfNull(src);
        ArgumentOutOfRangeException.ThrowIfLessThan(batchSize, 1);

        return BatchIterator(src, batchSize);

        static IEnumerable<T[]> BatchIterator(IEnumerable<T> source, int size)
        {
            var batch = new List<T>(size);
            foreach (var item in source)
            {
                batch.Add(item);
                if (batch.Count == size)
                {
                    yield return batch.ToArray();
                    batch.Clear();
                }
            }
            if (batch.Count > 0)
                yield return batch.ToArray();
        }
    }

    /// <summary>
    /// Executes the specified action on each element in the enumerable.
    /// </summary>
    /// <typeparam name="T">The element type</typeparam>
    /// <param name="src">The source enumerable</param>
    /// <param name="action">The action to execute on each element</param>
    public static void ForEach<T>(this IEnumerable<T> src, Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(src);
        ArgumentNullException.ThrowIfNull(action);

        foreach (var item in src)
            action(item);
    }

    /// <summary>
    /// Executes the specified action on each element along with its index.
    /// </summary>
    /// <typeparam name="T">The element type</typeparam>
    /// <param name="src">The source enumerable</param>
    /// <param name="action">The action to execute, receiving the element and its zero-based index</param>
    public static void ForEach<T>(this IEnumerable<T> src, Action<T, int> action)
    {
        ArgumentNullException.ThrowIfNull(src);
        ArgumentNullException.ThrowIfNull(action);

        int index = 0;
        foreach (var item in src)
            action(item, index++);
    }

    /// <summary>
    /// Interleaves elements from two sequences, taking one element from each in turn.
    /// If one sequence is longer, its remaining elements are appended at the end.
    /// </summary>
    /// <typeparam name="T">The element type</typeparam>
    /// <param name="first">The first sequence</param>
    /// <param name="second">The second sequence</param>
    /// <returns>The interleaved sequence</returns>
    public static IEnumerable<T> Interleave<T>(this IEnumerable<T> first, IEnumerable<T> second)
    {
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);

        return InterleaveIterator(first, second);

        static IEnumerable<T> InterleaveIterator(IEnumerable<T> a, IEnumerable<T> b)
        {
            using var e1 = a.GetEnumerator();
            using var e2 = b.GetEnumerator();
            bool has1 = e1.MoveNext(), has2 = e2.MoveNext();

            while (has1 || has2)
            {
                if (has1) { yield return e1.Current; has1 = e1.MoveNext(); }
                if (has2) { yield return e2.Current; has2 = e2.MoveNext(); }
            }
        }
    }

    /// <summary>
    /// Partitions the source into two lists based on a predicate.
    /// The first list contains elements matching the predicate, the second contains the rest.
    /// </summary>
    /// <typeparam name="T">The element type</typeparam>
    /// <param name="src">The source enumerable</param>
    /// <param name="predicate">The partitioning predicate</param>
    /// <returns>A tuple of (matching, nonMatching) lists</returns>
    public static (List<T> Matching, List<T> NonMatching) Partition<T>(this IEnumerable<T> src, Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(src);
        ArgumentNullException.ThrowIfNull(predicate);

        var matching = new List<T>();
        var nonMatching = new List<T>();

        foreach (var item in src)
        {
            if (predicate(item))
                matching.Add(item);
            else
                nonMatching.Add(item);
        }

        return (matching, nonMatching);
    }

    /// <summary>
    /// Applies an accumulator function over a sequence, yielding each intermediate result.
    /// Similar to LINQ's Aggregate but returns all intermediate accumulated values.
    /// </summary>
    /// <typeparam name="T">The element type</typeparam>
    /// <typeparam name="TAccumulate">The accumulator type</typeparam>
    /// <param name="src">The source enumerable</param>
    /// <param name="seed">The initial accumulator value</param>
    /// <param name="func">The accumulator function</param>
    /// <returns>The sequence of accumulated values</returns>
    public static IEnumerable<TAccumulate> Scan<T, TAccumulate>(
        this IEnumerable<T> src, TAccumulate seed, Func<TAccumulate, T, TAccumulate> func)
    {
        ArgumentNullException.ThrowIfNull(src);
        ArgumentNullException.ThrowIfNull(func);

        return ScanIterator(src, seed, func);

        static IEnumerable<TAccumulate> ScanIterator(IEnumerable<T> source, TAccumulate s, Func<TAccumulate, T, TAccumulate> f)
        {
            var accumulator = s;
            foreach (var item in source)
            {
                accumulator = f(accumulator, item);
                yield return accumulator;
            }
        }
    }

    /// <summary>
    /// Returns a sliding window of the specified size over the source sequence.
    /// Each window is an array of exactly <paramref name="windowSize"/> elements.
    /// </summary>
    /// <typeparam name="T">The element type</typeparam>
    /// <param name="src">The source enumerable</param>
    /// <param name="windowSize">The number of elements in each window</param>
    /// <returns>An enumerable of fixed-size windows</returns>
    public static IEnumerable<T[]> Window<T>(this IEnumerable<T> src, int windowSize)
    {
        ArgumentNullException.ThrowIfNull(src);
        ArgumentOutOfRangeException.ThrowIfLessThan(windowSize, 1);

        return WindowIterator(src, windowSize);

        static IEnumerable<T[]> WindowIterator(IEnumerable<T> source, int size)
        {
            var buffer = new Queue<T>(size);
            foreach (var item in source)
            {
                buffer.Enqueue(item);
                if (buffer.Count == size)
                {
                    yield return buffer.ToArray();
                    buffer.Dequeue();
                }
            }
        }
    }

    /// <summary>
    /// Removes consecutive duplicate elements from the sequence.
    /// Only removes duplicates that are adjacent — non-adjacent duplicates are preserved.
    /// </summary>
    /// <typeparam name="T">The element type</typeparam>
    /// <param name="src">The source enumerable</param>
    /// <returns>The sequence with consecutive duplicates removed</returns>
    public static IEnumerable<T> DistinctConsecutive<T>(this IEnumerable<T> src)
    {
        ArgumentNullException.ThrowIfNull(src);

        return DistinctConsecutiveIterator(src);

        static IEnumerable<T> DistinctConsecutiveIterator(IEnumerable<T> source)
        {
            var comparer = EqualityComparer<T>.Default;
            bool isFirst = true;
            T? previous = default;

            foreach (var item in source)
            {
                if (isFirst || !comparer.Equals(previous!, item))
                {
                    yield return item;
                    previous = item;
                    isFirst = false;
                }
            }
        }
    }

    /// <summary>
    /// Takes every Nth element from the sequence, starting with the first element.
    /// </summary>
    /// <typeparam name="T">The element type</typeparam>
    /// <param name="src">The source enumerable</param>
    /// <param name="step">The step size (must be >= 1)</param>
    /// <returns>Every Nth element</returns>
    public static IEnumerable<T> TakeEvery<T>(this IEnumerable<T> src, int step)
    {
        ArgumentNullException.ThrowIfNull(src);
        ArgumentOutOfRangeException.ThrowIfLessThan(step, 1);

        return TakeEveryIterator(src, step);

        static IEnumerable<T> TakeEveryIterator(IEnumerable<T> source, int s)
        {
            int index = 0;
            foreach (var item in source)
            {
                if (index % s == 0)
                    yield return item;
                index++;
            }
        }
    }

    /// <summary>
    /// Flattens a sequence of sequences into a single sequence.
    /// </summary>
    /// <typeparam name="T">The element type</typeparam>
    /// <param name="src">The source of enumerables</param>
    /// <returns>A flattened sequence</returns>
    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> src)
    {
        ArgumentNullException.ThrowIfNull(src);

        return FlattenIterator(src);

        static IEnumerable<T> FlattenIterator(IEnumerable<IEnumerable<T>> source)
        {
            foreach (var inner in source)
            {
                if (inner is null) continue;
                foreach (var item in inner)
                    yield return item;
            }
        }
    }

    /// <summary>
    /// Returns a specific page of results from the source enumerable.
    /// </summary>
    /// <typeparam name="T">The element type</typeparam>
    /// <param name="src">The source enumerable</param>
    /// <param name="pageNumber">The 1-based page number</param>
    /// <param name="pageSize">The number of elements per page</param>
    /// <returns>The elements on the specified page</returns>
    public static IEnumerable<T> Paginate<T>(this IEnumerable<T> src, int pageNumber, int pageSize)
    {
        ArgumentNullException.ThrowIfNull(src);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageNumber, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);

        return src.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }

    /// <summary>
    /// Returns the element at the specified index, or a default value if the index is out of range.
    /// </summary>
    /// <typeparam name="T">The element type</typeparam>
    /// <param name="src">The source enumerable</param>
    /// <param name="index">The zero-based index</param>
    /// <param name="defaultValue">The default value to return if index is out of range</param>
    /// <returns>The element at the index or the default value</returns>
    public static T SafeElementAt<T>(this IEnumerable<T> src, int index, T defaultValue = default!)
    {
        ArgumentNullException.ThrowIfNull(src);

        if (index < 0) return defaultValue;

        if (src is IList<T> list)
            return index < list.Count ? list[index] : defaultValue;

        int current = 0;
        foreach (var item in src)
        {
            if (current == index) return item;
            current++;
        }
        return defaultValue;
    }

    /// <summary>
    /// Returns the minimum and maximum values from the sequence in a single pass.
    /// </summary>
    /// <typeparam name="T">The element type, must be comparable</typeparam>
    /// <param name="src">The source enumerable</param>
    /// <returns>A tuple containing the minimum and maximum values</returns>
    public static (T Min, T Max) MinMax<T>(this IEnumerable<T> src) where T : IComparable<T>
    {
        ArgumentNullException.ThrowIfNull(src);

        using var enumerator = src.GetEnumerator();
        if (!enumerator.MoveNext())
            throw new InvalidOperationException("Sequence contains no elements.");

        var min = enumerator.Current;
        var max = enumerator.Current;

        while (enumerator.MoveNext())
        {
            if (enumerator.Current.CompareTo(min) < 0) min = enumerator.Current;
            if (enumerator.Current.CompareTo(max) > 0) max = enumerator.Current;
        }

        return (min, max);
    }

    /// <summary>
    /// Returns the sequence without null values (for reference types and nullable value types).
    /// </summary>
    /// <typeparam name="T">The element type</typeparam>
    /// <param name="src">The source enumerable</param>
    /// <returns>The source with nulls removed</returns>
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> src) where T : class
    {
        ArgumentNullException.ThrowIfNull(src);
        return src.Where(x => x is not null)!;
    }

    /// <summary>
    /// Returns a sequence with duplicate elements removed, keeping only the first occurrence.
    /// Uses the specified key selector to determine uniqueness.
    /// </summary>
    /// <typeparam name="T">The element type</typeparam>
    /// <typeparam name="TKey">The key type</typeparam>
    /// <param name="src">The source enumerable</param>
    /// <param name="keySelector">The function to extract the comparison key</param>
    /// <returns>The distinct sequence</returns>
    public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> src, Func<T, TKey> keySelector)
    {
        ArgumentNullException.ThrowIfNull(src);
        ArgumentNullException.ThrowIfNull(keySelector);

        return DistinctByIterator(src, keySelector);

        static IEnumerable<T> DistinctByIterator(IEnumerable<T> source, Func<T, TKey> selector)
        {
            var seen = new HashSet<TKey>();
            foreach (var item in source)
            {
                if (seen.Add(selector(item)))
                    yield return item;
            }
        }
    }

    /// <summary>
    /// Groups elements by a key and returns each group as a key-count pair.
    /// </summary>
    /// <typeparam name="T">The element type</typeparam>
    /// <typeparam name="TKey">The key type</typeparam>
    /// <param name="src">The source enumerable</param>
    /// <param name="keySelector">The function to extract the grouping key</param>
    /// <returns>A dictionary mapping keys to their occurrence counts</returns>
    public static Dictionary<TKey, int> CountBy<T, TKey>(this IEnumerable<T> src, Func<T, TKey> keySelector)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(src);
        ArgumentNullException.ThrowIfNull(keySelector);

        var counts = new Dictionary<TKey, int>();
        foreach (var item in src)
        {
            var key = keySelector(item);
            if (!counts.TryAdd(key, 1))
                counts[key]++;
        }
        return counts;
    }
}
