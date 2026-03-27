using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

#nullable enable

namespace Ampere.Base;

/// <summary>
/// Provides cryptographically secure shuffling and sampling algorithms.
/// All methods use <see cref="RandomNumberGenerator"/> for unbiased randomness.
/// </summary>
public static class Shuffler
{
    /// <summary>
    /// Performs an in-place Fisher–Yates shuffle on an array using
    /// cryptographically secure random number generation.
    /// </summary>
    /// <typeparam name="T">The element type</typeparam>
    /// <param name="array">The array to shuffle in-place</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Shuffle<T>(T[] array)
    {
        ArgumentNullException.ThrowIfNull(array);
        Shuffle(array.AsSpan());
    }

    /// <summary>
    /// Performs an in-place Fisher–Yates shuffle on a span using
    /// cryptographically secure random number generation.
    /// This is the zero-allocation core implementation.
    /// </summary>
    /// <typeparam name="T">The element type</typeparam>
    /// <param name="span">The span to shuffle in-place</param>
    public static void Shuffle<T>(Span<T> span)
    {
        for (int i = span.Length - 1; i > 0; i--)
        {
            int j = RandomNumberGenerator.GetInt32(i + 1);
            (span[i], span[j]) = (span[j], span[i]);
        }
    }

    /// <summary>
    /// Performs an in-place Fisher–Yates shuffle on an <see cref="IList{T}"/>.
    /// </summary>
    /// <typeparam name="T">The element type</typeparam>
    /// <param name="list">The list to shuffle in-place</param>
    public static void Shuffle<T>(IList<T> list)
    {
        ArgumentNullException.ThrowIfNull(list);
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = RandomNumberGenerator.GetInt32(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    /// <summary>
    /// Returns a new shuffled copy of the source array without
    /// modifying the original (non-destructive).
    /// </summary>
    /// <typeparam name="T">The element type</typeparam>
    /// <param name="source">The source array</param>
    /// <returns>A new array with elements in random order</returns>
    public static T[] ShuffleCopy<T>(ReadOnlySpan<T> source)
    {
        var copy = source.ToArray();
        Shuffle(copy.AsSpan());
        return copy;
    }

    /// <summary>
    /// Returns a new shuffled copy of the source enumerable.
    /// </summary>
    /// <typeparam name="T">The element type</typeparam>
    /// <param name="source">The source enumerable</param>
    /// <returns>A new array with elements in random order</returns>
    public static T[] ShuffleCopy<T>(IEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);
        var array = source is T[] arr ? (T[])arr.Clone() : [.. source];
        Shuffle(array.AsSpan());
        return array;
    }

    /// <summary>
    /// Selects <paramref name="k"/> random elements from the source using a
    /// partial Fisher–Yates shuffle. Only the first k positions are shuffled,
    /// making this O(k) instead of O(n).
    /// </summary>
    /// <typeparam name="T">The element type</typeparam>
    /// <param name="source">The source array</param>
    /// <param name="k">The number of elements to select</param>
    /// <returns>An array of k randomly selected elements</returns>
    public static T[] PartialShuffle<T>(ReadOnlySpan<T> source, int k)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(k);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(k, source.Length);

        var work = source.ToArray();
        for (int i = 0; i < k; i++)
        {
            int j = RandomNumberGenerator.GetInt32(i, work.Length);
            (work[i], work[j]) = (work[j], work[i]);
        }

        return work.AsSpan(0, k).ToArray();
    }

    /// <summary>
    /// Performs reservoir sampling (Vitter's Algorithm L) to select
    /// <paramref name="k"/> elements uniformly at random from a potentially
    /// infinite or very large stream. Runs in O(n) time with O(k) space.
    /// </summary>
    /// <typeparam name="T">The element type</typeparam>
    /// <param name="source">The source enumerable (may be streamed)</param>
    /// <param name="k">The reservoir size (number of elements to sample)</param>
    /// <returns>An array of k uniformly sampled elements</returns>
    public static T[] ReservoirSample<T>(IEnumerable<T> source, int k)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(k);

        var reservoir = new T[k];
        int index = 0;

        using var enumerator = source.GetEnumerator();

        // Fill the reservoir with the first k elements
        while (index < k)
        {
            if (!enumerator.MoveNext())
                throw new ArgumentException($"Source has fewer than {k} elements.", nameof(source));
            reservoir[index++] = enumerator.Current;
        }

        // Algorithm L: exponential jumps for efficiency
        double w = Math.Exp(Math.Log(RandomDouble()) / k);

        while (true)
        {
            // Compute the number of elements to skip
            int skip = (int)Math.Floor(Math.Log(RandomDouble()) / Math.Log(1.0 - w)) + 1;

            for (int s = 0; s < skip; s++)
            {
                if (!enumerator.MoveNext()) return reservoir;
                index++;
            }

            reservoir[RandomNumberGenerator.GetInt32(k)] = enumerator.Current;
            w *= Math.Exp(Math.Log(RandomDouble()) / k);
        }
    }

    /// <summary>
    /// Selects <paramref name="k"/> elements from a weighted distribution using
    /// the Efraimidis–Spirakis algorithm. Each element is associated with a weight
    /// that determines its probability of selection. Runs in O(n log k).
    /// </summary>
    /// <typeparam name="T">The element type</typeparam>
    /// <param name="items">The source items</param>
    /// <param name="weights">The corresponding weights (must be positive)</param>
    /// <param name="k">The number of elements to select</param>
    /// <returns>An array of k weighted-sampled elements</returns>
    public static T[] WeightedSample<T>(ReadOnlySpan<T> items, ReadOnlySpan<double> weights, int k)
    {
        if (items.Length != weights.Length)
            throw new ArgumentException("Items and weights must have the same length.");
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(k);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(k, items.Length);

        // Compute keys: u^(1/w) for each element using the Efraimidis-Spirakis method
        var keys = new (double Key, int Index)[items.Length];
        for (int i = 0; i < items.Length; i++)
        {
            if (weights[i] <= 0)
                throw new ArgumentException($"Weight at index {i} must be positive.");
            keys[i] = (Math.Pow(RandomDouble(), 1.0 / weights[i]), i);
        }

        // Selection: top-k by key value
        Array.Sort(keys, (a, b) => b.Key.CompareTo(a.Key));

        var result = new T[k];
        for (int i = 0; i < k; i++)
            result[i] = items[keys[i].Index];
        return result;
    }

    /// <summary>
    /// Produces a random derangement of the given array using Sattolo's algorithm.
    /// A derangement is a permutation where no element appears in its original position.
    /// Returns a new array; the original is not modified.
    /// </summary>
    /// <typeparam name="T">The element type</typeparam>
    /// <param name="source">The source array (must have at least 2 elements)</param>
    /// <returns>A new array representing a random derangement of the source</returns>
    public static T[] Derange<T>(ReadOnlySpan<T> source)
    {
        if (source.Length < 2)
            throw new ArgumentException("Derangement requires at least 2 elements.", nameof(source));

        var result = source.ToArray();
        // Sattolo's algorithm: guaranteed cycle of length n (no fixed points)
        for (int i = result.Length - 1; i > 0; i--)
        {
            int j = RandomNumberGenerator.GetInt32(i); // [0, i) — excludes i
            (result[i], result[j]) = (result[j], result[i]);
        }
        return result;
    }

    /// <summary>
    /// Generates a uniformly distributed random double in (0, 1) exclusive
    /// using cryptographic randomness. Uses 53 bits of entropy to fill
    /// the full mantissa of a double-precision float.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double RandomDouble()
    {
        Span<byte> bytes = stackalloc byte[8]; //I used this as an exucse to finally use stackalloc
        RandomNumberGenerator.Fill(bytes);
        ulong value = BitConverter.ToUInt64(bytes) >> 11; // 53 bits
        // Map to (0, 1) exclusive of 0 by adding 1 then dividing by 2^53 + 1
        return (value + 1.0) / (1UL << 53);
    }
}
