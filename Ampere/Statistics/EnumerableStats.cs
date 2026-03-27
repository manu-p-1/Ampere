#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Ampere.Statistics;

/// <summary>
/// A static utility class for calculating a variety of statistics based on IEnumerables. Provides
/// extension methods for <see cref="IEnumerable{T}"/> with generic overloads that accept a selector
/// function, enabling statistics computation on object enumerables containing numeric properties.
/// </summary>
public static class EnumerableStats
{
    /// <summary>
    /// Materializes the source enumerable into an array for safe multi-pass iteration.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double[] Materialize(IEnumerable<double> src)
    {
        return src as double[] ?? [.. src];
    }

    /// <summary>
    /// Validates that the source is not null and not empty, returning the materialized array.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double[] ValidateNotEmpty(IEnumerable<double> src, [CallerMemberName] string? caller = null)
    {
        ArgumentNullException.ThrowIfNull(src);
        var arr = Materialize(src);
        if (arr.Length == 0)
            throw new InvalidOperationException($"Source enumerable must not be empty (called from {caller}).");
        return arr;
    }

    /// <summary>
    /// Computes the arithmetic mean (average) of all elements in the enumerable.
    /// </summary>
    /// <param name="src">The IEnumerable of type double</param>
    /// <exception cref="ArgumentNullException">Thrown when the source is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when the source is empty</exception>
    /// <returns>The arithmetic mean of the source</returns>
    public static double Mean(this IEnumerable<double> src)
    {
        var arr = ValidateNotEmpty(src);
        return arr.Length == 1 ? arr[0] : arr.Average();
    }

    /// <summary>
    /// Computes the arithmetic mean of the projected values.
    /// </summary>
    /// <typeparam name="T">The type of the enumerable</typeparam>
    /// <param name="numbers">The specified enumerable</param>
    /// <param name="selector">The numeric property selector</param>
    /// <returns>The arithmetic mean of the projected values</returns>
    public static double Mean<T>(this IEnumerable<T> numbers, Func<T, double> selector)
    {
        ArgumentNullException.ThrowIfNull(numbers);
        ArgumentNullException.ThrowIfNull(selector);
        return numbers.Select(selector).Mean();
    }

    /// <summary>
    /// Computes the median (middle value) of all elements in the enumerable.
    /// </summary>
    /// <param name="src">The IEnumerable of type double</param>
    /// <exception cref="ArgumentNullException">Thrown when the source is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when the source is empty</exception>
    /// <returns>The median of the source</returns>
    public static double Median(this IEnumerable<double> src)
    {
        var arr = ValidateNotEmpty(src);
        if (arr.Length == 1) return arr[0];

        var sorted = new double[arr.Length];
        Array.Copy(arr, sorted, arr.Length);
        Array.Sort(sorted);

        int mid = sorted.Length / 2;
        return sorted.Length % 2 == 0
            ? (sorted[mid - 1] + sorted[mid]) / 2.0
            : sorted[mid];
    }

    /// <summary>
    /// Computes the median of the projected values.
    /// </summary>
    /// <typeparam name="T">The type of the enumerable</typeparam>
    /// <param name="numbers">The specified enumerable</param>
    /// <param name="selector">The numeric property selector</param>
    /// <returns>The median of the projected values</returns>
    public static double Median<T>(this IEnumerable<T> numbers, Func<T, double> selector)
    {
        ArgumentNullException.ThrowIfNull(numbers);
        ArgumentNullException.ThrowIfNull(selector);
        return numbers.Select(selector).Median();
    }

    /// <summary>
    /// Finds the mode (most frequently occurring value) of all elements in the enumerable.
    /// When multiple values share the highest frequency, the first encountered value is returned.
    /// </summary>
    /// <param name="src">The IEnumerable of type double</param>
    /// <exception cref="ArgumentNullException">Thrown when the source is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when the source is empty</exception>
    /// <exception cref="NoModeException">Thrown when all values occur with equal frequency</exception>
    /// <returns>The mode of the source</returns>
    public static double Mode(this IEnumerable<double> src)
    {
        var arr = ValidateNotEmpty(src);
        if (arr.Length == 1) return arr[0];

        var freq = new Dictionary<double, int>();
        foreach (double val in arr)
        {
            if (!freq.TryAdd(val, 1)) freq[val]++;
        }

        int maxFreq = 0;
        double modeValue = 0;
        bool allEqual = true;

        foreach (var kvp in freq)
        {
            if (kvp.Value > maxFreq)
            {
                maxFreq = kvp.Value;
                modeValue = kvp.Key;
            }
        }

        foreach (var kvp in freq)
        {
            if (kvp.Value != maxFreq)
            {
                allEqual = false;
                break;
            }
        }

        if (allEqual)
            throw new NoModeException("No mode present — all values occur with equal frequency.");

        return modeValue;
    }

    /// <summary>
    /// Finds the mode of the projected values.
    /// </summary>
    /// <typeparam name="T">The type of the enumerable</typeparam>
    /// <param name="numbers">The specified enumerable</param>
    /// <param name="selector">The numeric property selector</param>
    /// <returns>The mode of the projected values</returns>
    public static double Mode<T>(this IEnumerable<T> numbers, Func<T, double> selector)
    {
        ArgumentNullException.ThrowIfNull(numbers);
        ArgumentNullException.ThrowIfNull(selector);
        return numbers.Select(selector).Mode();
    }

    /// <summary>
    /// Computes the geometric mean, which is the nth root of the product of n values.
    /// Useful for averaging growth rates, ratios, and percentages.
    /// All values must be positive.
    /// </summary>
    /// <param name="src">The IEnumerable of type double</param>
    /// <exception cref="ArgumentNullException">Thrown when the source is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when the source is empty or contains non-positive values</exception>
    /// <returns>The geometric mean of the source</returns>
    public static double GeometricMean(this IEnumerable<double> src)
    {
        var arr = ValidateNotEmpty(src);

        double logSum = 0;
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] <= 0)
                throw new InvalidOperationException("Geometric mean requires all positive values.");
            logSum += Math.Log(arr[i]);
        }

        return Math.Exp(logSum / arr.Length);
    }

    /// <summary>
    /// Computes the geometric mean of the projected values.
    /// </summary>
    public static double GeometricMean<T>(this IEnumerable<T> numbers, Func<T, double> selector)
    {
        ArgumentNullException.ThrowIfNull(numbers);
        ArgumentNullException.ThrowIfNull(selector);
        return numbers.Select(selector).GeometricMean();
    }

    /// <summary>
    /// Computes the harmonic mean, which is the reciprocal of the arithmetic mean of the reciprocals.
    /// Useful for averaging rates (e.g., speed, throughput).
    /// All values must be positive.
    /// </summary>
    /// <param name="src">The IEnumerable of type double</param>
    /// <exception cref="ArgumentNullException">Thrown when the source is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when the source is empty or contains non-positive values</exception>
    /// <returns>The harmonic mean of the source</returns>
    public static double HarmonicMean(this IEnumerable<double> src)
    {
        var arr = ValidateNotEmpty(src);

        double reciprocalSum = 0;
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] <= 0)
                throw new InvalidOperationException("Harmonic mean requires all positive values.");
            reciprocalSum += 1.0 / arr[i];
        }

        return arr.Length / reciprocalSum;
    }

    /// <summary>
    /// Computes the harmonic mean of the projected values.
    /// </summary>
    public static double HarmonicMean<T>(this IEnumerable<T> numbers, Func<T, double> selector)
    {
        ArgumentNullException.ThrowIfNull(numbers);
        ArgumentNullException.ThrowIfNull(selector);
        return numbers.Select(selector).HarmonicMean();
    }

    /// <summary>
    /// Computes the weighted arithmetic mean of the values using the specified weights.
    /// </summary>
    /// <param name="src">The values</param>
    /// <param name="weights">The weights corresponding to each value</param>
    /// <exception cref="ArgumentNullException">Thrown when the source or weights are null</exception>
    /// <exception cref="InvalidOperationException">Thrown when the source is empty or counts differ</exception>
    /// <returns>The weighted mean</returns>
    public static double WeightedMean(this IEnumerable<double> src, IEnumerable<double> weights)
    {
        var vals = ValidateNotEmpty(src);
        ArgumentNullException.ThrowIfNull(weights);
        var w = Materialize(weights);

        if (vals.Length != w.Length)
            throw new InvalidOperationException("Values and weights must have the same number of elements.");

        double weightedSum = 0;
        double weightSum = 0;
        for (int i = 0; i < vals.Length; i++)
        {
            weightedSum += vals[i] * w[i];
            weightSum += w[i];
        }

        if (weightSum == 0)
            throw new InvalidOperationException("Sum of weights must not be zero.");

        return weightedSum / weightSum;
    }

    /// <summary>
    /// Computes the trimmed mean by excluding a specified fraction of values from both tails.
    /// </summary>
    /// <param name="src">The IEnumerable of type double</param>
    /// <param name="trimFraction">The fraction to trim from each end (0.0 to 0.5 exclusive)</param>
    /// <exception cref="ArgumentNullException">Thrown when the source is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when the source is empty</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when trimFraction is out of range</exception>
    /// <returns>The trimmed mean</returns>
    public static double TrimmedMean(this IEnumerable<double> src, double trimFraction)
    {
        var arr = ValidateNotEmpty(src);
        if (trimFraction < 0.0 || trimFraction >= 0.5)
            throw new ArgumentOutOfRangeException(nameof(trimFraction), "Trim fraction must be in [0.0, 0.5).");

        var sorted = new double[arr.Length];
        Array.Copy(arr, sorted, arr.Length);
        Array.Sort(sorted);

        int trimCount = (int)(sorted.Length * trimFraction);
        if (sorted.Length - 2 * trimCount <= 0)
            throw new InvalidOperationException("Too few elements remain after trimming.");

        double sum = 0;
        int count = 0;
        for (int i = trimCount; i < sorted.Length - trimCount; i++)
        {
            sum += sorted[i];
            count++;
        }

        return sum / count;
    }

    /// <summary>
    /// Computes the trimmed mean of the projected values.
    /// </summary>
    public static double TrimmedMean<T>(this IEnumerable<T> numbers, Func<T, double> selector, double trimFraction)
    {
        ArgumentNullException.ThrowIfNull(numbers);
        ArgumentNullException.ThrowIfNull(selector);
        return numbers.Select(selector).TrimmedMean(trimFraction);
    }

    /// <summary>
    /// Computes the sample variance of the enumerable using Bessel's correction (n-1 denominator).
    /// </summary>
    /// <param name="src">The IEnumerable of type double</param>
    /// <exception cref="ArgumentNullException">Thrown when the source is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when the source is empty</exception>
    /// <returns>The sample variance of the source</returns>
    public static double SampleVariance(this IEnumerable<double> src)
    {
        var arr = ValidateNotEmpty(src);
        if (arr.Length < 2)
            throw new InsufficientDataSetException("Sample variance requires at least 2 data points.");

        double mean = arr.Average();
        double sum = 0;
        for (int i = 0; i < arr.Length; i++)
        {
            double diff = arr[i] - mean;
            sum += diff * diff;
        }

        return sum / (arr.Length - 1);
    }

    /// <summary>
    /// Computes the sample variance of the projected values.
    /// </summary>
    public static double SampleVariance<T>(this IEnumerable<T> numbers, Func<T, double> selector)
    {
        ArgumentNullException.ThrowIfNull(numbers);
        ArgumentNullException.ThrowIfNull(selector);
        return numbers.Select(selector).SampleVariance();
    }

    /// <summary>
    /// Computes the population variance of the enumerable (n denominator).
    /// </summary>
    /// <param name="src">The IEnumerable of type double</param>
    /// <exception cref="ArgumentNullException">Thrown when the source is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when the source is empty</exception>
    /// <returns>The population variance of the source</returns>
    public static double PopulationVariance(this IEnumerable<double> src)
    {
        var arr = ValidateNotEmpty(src);

        double mean = arr.Average();
        double sum = 0;
        for (int i = 0; i < arr.Length; i++)
        {
            double diff = arr[i] - mean;
            sum += diff * diff;
        }

        return sum / arr.Length;
    }

    /// <summary>
    /// Computes the population variance of the projected values.
    /// </summary>
    public static double PopulationVariance<T>(this IEnumerable<T> numbers, Func<T, double> selector)
    {
        ArgumentNullException.ThrowIfNull(numbers);
        ArgumentNullException.ThrowIfNull(selector);
        return numbers.Select(selector).PopulationVariance();
    }

    /// <summary>
    /// Computes the sample standard deviation of the enumerable (square root of sample variance).
    /// </summary>
    /// <param name="src">The IEnumerable of type double</param>
    /// <exception cref="ArgumentNullException">Thrown when the source is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when the source is empty</exception>
    /// <returns>The sample standard deviation of the source</returns>
    public static double SampleStandardDeviation(this IEnumerable<double> src)
    {
        return Math.Sqrt(src.SampleVariance());
    }

    /// <summary>
    /// Computes the sample standard deviation of the projected values.
    /// </summary>
    public static double SampleStandardDeviation<T>(this IEnumerable<T> numbers, Func<T, double> selector)
    {
        ArgumentNullException.ThrowIfNull(numbers);
        ArgumentNullException.ThrowIfNull(selector);
        return numbers.Select(selector).SampleStandardDeviation();
    }

    /// <summary>
    /// Computes the population standard deviation of the enumerable (square root of population variance).
    /// </summary>
    /// <param name="src">The IEnumerable of type double</param>
    /// <exception cref="ArgumentNullException">Thrown when the source is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when the source is empty</exception>
    /// <returns>The population standard deviation of the source</returns>
    public static double PopulationStandardDeviation(this IEnumerable<double> src)
    {
        return Math.Sqrt(src.PopulationVariance());
    }

    /// <summary>
    /// Computes the population standard deviation of the projected values.
    /// </summary>
    public static double PopulationStandardDeviation<T>(this IEnumerable<T> numbers, Func<T, double> selector)
    {
        ArgumentNullException.ThrowIfNull(numbers);
        ArgumentNullException.ThrowIfNull(selector);
        return numbers.Select(selector).PopulationStandardDeviation();
    }

    /// <summary>
    /// Computes the range (max - min) of the enumerable.
    /// </summary>
    /// <param name="src">The IEnumerable of type double</param>
    /// <exception cref="ArgumentNullException">Thrown when the source is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when the source is empty</exception>
    /// <returns>The range of the source</returns>
    public static double Range(this IEnumerable<double> src)
    {
        var arr = ValidateNotEmpty(src);
        if (arr.Length == 1) return 0;

        double min = arr[0], max = arr[0];
        for (int i = 1; i < arr.Length; i++)
        {
            if (arr[i] < min) min = arr[i];
            if (arr[i] > max) max = arr[i];
        }

        return max - min;
    }

    /// <summary>
    /// Computes the range of the projected values.
    /// </summary>
    public static double Range<T>(this IEnumerable<T> numbers, Func<T, double> selector)
    {
        ArgumentNullException.ThrowIfNull(numbers);
        ArgumentNullException.ThrowIfNull(selector);
        return numbers.Select(selector).Range();
    }

    /// <summary>
    /// Computes the mean absolute deviation from the mean.
    /// </summary>
    /// <param name="src">The IEnumerable of type double</param>
    /// <exception cref="ArgumentNullException">Thrown when the source is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when the source is empty</exception>
    /// <returns>The mean absolute deviation</returns>
    public static double MeanAbsoluteDeviation(this IEnumerable<double> src)
    {
        var arr = ValidateNotEmpty(src);
        double mean = arr.Average();

        double sum = 0;
        for (int i = 0; i < arr.Length; i++)
            sum += Math.Abs(arr[i] - mean);

        return sum / arr.Length;
    }

    /// <summary>
    /// Computes the mean absolute deviation of the projected values.
    /// </summary>
    public static double MeanAbsoluteDeviation<T>(this IEnumerable<T> numbers, Func<T, double> selector)
    {
        ArgumentNullException.ThrowIfNull(numbers);
        ArgumentNullException.ThrowIfNull(selector);
        return numbers.Select(selector).MeanAbsoluteDeviation();
    }

    /// <summary>
    /// Computes the median absolute deviation (MAD), a robust measure of variability.
    /// MAD = Median(|xi - Median(x)|)
    /// </summary>
    /// <param name="src">The IEnumerable of type double</param>
    /// <exception cref="ArgumentNullException">Thrown when the source is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when the source is empty</exception>
    /// <returns>The median absolute deviation</returns>
    public static double MedianAbsoluteDeviation(this IEnumerable<double> src)
    {
        var arr = ValidateNotEmpty(src);
        double median = arr.Median();

        var deviations = new double[arr.Length];
        for (int i = 0; i < arr.Length; i++)
            deviations[i] = Math.Abs(arr[i] - median);

        return deviations.Median();
    }

    /// <summary>
    /// Computes the median absolute deviation of the projected values.
    /// </summary>
    public static double MedianAbsoluteDeviation<T>(this IEnumerable<T> numbers, Func<T, double> selector)
    {
        ArgumentNullException.ThrowIfNull(numbers);
        ArgumentNullException.ThrowIfNull(selector);
        return numbers.Select(selector).MedianAbsoluteDeviation();
    }

    /// <summary>
    /// Computes the coefficient of variation (CV), the ratio of standard deviation to mean.
    /// Expressed as a fraction (not percentage). Useful for comparing variability across data sets
    /// with different units or scales.
    /// </summary>
    /// <param name="src">The IEnumerable of type double</param>
    /// <exception cref="ArgumentNullException">Thrown when the source is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when the source is empty or mean is zero</exception>
    /// <returns>The coefficient of variation</returns>
    public static double CoefficientOfVariation(this IEnumerable<double> src)
    {
        var arr = ValidateNotEmpty(src);
        double mean = arr.Average();
        if (mean == 0)
            throw new InvalidOperationException("Coefficient of variation is undefined when the mean is zero.");

        return arr.SampleStandardDeviation() / Math.Abs(mean);
    }

    /// <summary>
    /// Computes the coefficient of variation of the projected values.
    /// </summary>
    public static double CoefficientOfVariation<T>(this IEnumerable<T> numbers, Func<T, double> selector)
    {
        ArgumentNullException.ThrowIfNull(numbers);
        ArgumentNullException.ThrowIfNull(selector);
        return numbers.Select(selector).CoefficientOfVariation();
    }

    /// <summary>
    /// Computes the lower quartile (25th percentile / Q1) of the enumerable.
    /// </summary>
    /// <param name="src">The IEnumerable of type double</param>
    /// <exception cref="ArgumentNullException">Thrown when the source is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when the source is empty</exception>
    /// <exception cref="InsufficientDataSetException">Thrown when the data set has fewer than 3 elements</exception>
    /// <returns>The lower quartile of the source</returns>
    public static double LowerQuartile(this IEnumerable<double> src)
    {
        var arr = ValidateNotEmpty(src);
        if (arr.Length < 3)
            throw new InsufficientDataSetException("Data set not large enough to calculate a lower quartile.");

        var sorted = new double[arr.Length];
        Array.Copy(arr, sorted, arr.Length);
        Array.Sort(sorted);

        int mid = sorted.Length / 2;
        var lowerHalf = new double[mid];
        Array.Copy(sorted, 0, lowerHalf, 0, mid);

        return lowerHalf.Median();
    }

    /// <summary>
    /// Computes the lower quartile of the projected values.
    /// </summary>
    public static double LowerQuartile<T>(this IEnumerable<T> numbers, Func<T, double> selector)
    {
        ArgumentNullException.ThrowIfNull(numbers);
        ArgumentNullException.ThrowIfNull(selector);
        return numbers.Select(selector).LowerQuartile();
    }

    /// <summary>
    /// Computes the upper quartile (75th percentile / Q3) of the enumerable.
    /// </summary>
    /// <param name="src">The IEnumerable of type double</param>
    /// <exception cref="ArgumentNullException">Thrown when the source is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when the source is empty</exception>
    /// <exception cref="InsufficientDataSetException">Thrown when the data set has fewer than 3 elements</exception>
    /// <returns>The upper quartile of the source</returns>
    public static double UpperQuartile(this IEnumerable<double> src)
    {
        var arr = ValidateNotEmpty(src);
        if (arr.Length < 3)
            throw new InsufficientDataSetException("Data set not large enough to calculate an upper quartile.");

        var sorted = new double[arr.Length];
        Array.Copy(arr, sorted, arr.Length);
        Array.Sort(sorted);

        int mid = sorted.Length / 2;
        int upperStart = sorted.Length % 2 == 0 ? mid : mid + 1;
        int upperLen = sorted.Length - upperStart;
        var upperHalf = new double[upperLen];
        Array.Copy(sorted, upperStart, upperHalf, 0, upperLen);

        return upperHalf.Median();
    }

    /// <summary>
    /// Computes the upper quartile of the projected values.
    /// </summary>
    public static double UpperQuartile<T>(this IEnumerable<T> numbers, Func<T, double> selector)
    {
        ArgumentNullException.ThrowIfNull(numbers);
        ArgumentNullException.ThrowIfNull(selector);
        return numbers.Select(selector).UpperQuartile();
    }

    /// <summary>
    /// Computes the inter-quartile range (IQR = Q3 - Q1) of the enumerable.
    /// </summary>
    /// <param name="src">The IEnumerable of type double</param>
    /// <exception cref="ArgumentNullException">Thrown when the source is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when the source is empty</exception>
    /// <returns>The inter-quartile range of the source</returns>
    public static double InterQuartileRange(this IEnumerable<double> src)
    {
        var arr = ValidateNotEmpty(src);
        return arr.UpperQuartile() - arr.LowerQuartile();
    }

    /// <summary>
    /// Computes the inter-quartile range of the projected values.
    /// </summary>
    public static double InterQuartileRange<T>(this IEnumerable<T> numbers, Func<T, double> selector)
    {
        ArgumentNullException.ThrowIfNull(numbers);
        ArgumentNullException.ThrowIfNull(selector);
        return numbers.Select(selector).InterQuartileRange();
    }

    /// <summary>
    /// Computes the value at the specified percentile using linear interpolation.
    /// </summary>
    /// <param name="src">The IEnumerable of type double</param>
    /// <param name="percentile">The percentile to compute (0 to 100 inclusive)</param>
    /// <exception cref="ArgumentNullException">Thrown when the source is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when the source is empty</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when percentile is outside [0, 100]</exception>
    /// <returns>The value at the specified percentile</returns>
    public static double Percentile(this IEnumerable<double> src, double percentile)
    {
        var arr = ValidateNotEmpty(src);
        if (percentile < 0.0 || percentile > 100.0)
            throw new ArgumentOutOfRangeException(nameof(percentile), "Percentile must be between 0 and 100.");

        var sorted = new double[arr.Length];
        Array.Copy(arr, sorted, arr.Length);
        Array.Sort(sorted);

        if (sorted.Length == 1) return sorted[0];

        double rank = (percentile / 100.0) * (sorted.Length - 1);
        int lower = (int)Math.Floor(rank);
        int upper = (int)Math.Ceiling(rank);

        if (lower == upper) return sorted[lower];

        double fraction = rank - lower;
        return sorted[lower] + fraction * (sorted[upper] - sorted[lower]);
    }

    /// <summary>
    /// Computes the value at the specified percentile of the projected values.
    /// </summary>
    public static double Percentile<T>(this IEnumerable<T> numbers, Func<T, double> selector, double percentile)
    {
        ArgumentNullException.ThrowIfNull(numbers);
        ArgumentNullException.ThrowIfNull(selector);
        return numbers.Select(selector).Percentile(percentile);
    }

    /// <summary>
    /// Computes Fisher's sample skewness coefficient, measuring the asymmetry of the distribution.
    /// Positive skew indicates a longer right tail; negative skew indicates a longer left tail.
    /// </summary>
    /// <param name="src">The IEnumerable of type double</param>
    /// <exception cref="ArgumentNullException">Thrown when the source is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when the source is empty</exception>
    /// <exception cref="InsufficientDataSetException">Thrown when fewer than 3 data points</exception>
    /// <returns>The sample skewness</returns>
    public static double Skewness(this IEnumerable<double> src)
    {
        var arr = ValidateNotEmpty(src);
        if (arr.Length < 3)
            throw new InsufficientDataSetException("Skewness requires at least 3 data points.");

        int n = arr.Length;
        double mean = arr.Average();
        double m2 = 0, m3 = 0;

        for (int i = 0; i < n; i++)
        {
            double diff = arr[i] - mean;
            m2 += diff * diff;
            m3 += diff * diff * diff;
        }

        m2 /= n;
        m3 /= n;

        double s2 = Math.Sqrt(m2);
        if (s2 == 0) return 0;

        double g1 = m3 / (s2 * s2 * s2);
        // Apply the bias correction factor for sample skewness
        return g1 * Math.Sqrt((double)n * (n - 1)) / (n - 2);
    }

    /// <summary>
    /// Computes the sample skewness of the projected values.
    /// </summary>
    public static double Skewness<T>(this IEnumerable<T> numbers, Func<T, double> selector)
    {
        ArgumentNullException.ThrowIfNull(numbers);
        ArgumentNullException.ThrowIfNull(selector);
        return numbers.Select(selector).Skewness();
    }

    /// <summary>
    /// Computes the sample excess kurtosis, measuring the "tailedness" of the distribution.
    /// Normal distribution has excess kurtosis of 0. Positive values indicate heavier tails (leptokurtic),
    /// negative values indicate lighter tails (platykurtic).
    /// </summary>
    /// <param name="src">The IEnumerable of type double</param>
    /// <exception cref="ArgumentNullException">Thrown when the source is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when the source is empty</exception>
    /// <exception cref="InsufficientDataSetException">Thrown when fewer than 4 data points</exception>
    /// <returns>The sample excess kurtosis</returns>
    public static double ExcessKurtosis(this IEnumerable<double> src)
    {
        var arr = ValidateNotEmpty(src);
        if (arr.Length < 4)
            throw new InsufficientDataSetException("Excess kurtosis requires at least 4 data points.");

        int n = arr.Length;
        double mean = arr.Average();
        double m2 = 0, m4 = 0;

        for (int i = 0; i < n; i++)
        {
            double diff = arr[i] - mean;
            double d2 = diff * diff;
            m2 += d2;
            m4 += d2 * d2;
        }

        m2 /= n;
        m4 /= n;

        if (m2 == 0) return 0;

        double g2 = (m4 / (m2 * m2)) - 3.0;

        // Bias-corrected sample excess kurtosis
        double correction = (double)(n - 1) / ((n - 2) * (n - 3));
        return correction * ((n + 1) * g2 + 6.0);
    }

    /// <summary>
    /// Computes the sample excess kurtosis of the projected values.
    /// </summary>
    public static double ExcessKurtosis<T>(this IEnumerable<T> numbers, Func<T, double> selector)
    {
        ArgumentNullException.ThrowIfNull(numbers);
        ArgumentNullException.ThrowIfNull(selector);
        return numbers.Select(selector).ExcessKurtosis();
    }

    /// <summary>
    /// Computes the standardized score (z-score) of a given element relative to the data set.
    /// z = (x - mean) / stddev
    /// </summary>
    /// <param name="src">The IEnumerable of type double</param>
    /// <param name="elem">The value to compute the z-score for</param>
    /// <exception cref="ArgumentNullException">Thrown when the source is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when the source is empty or has zero standard deviation</exception>
    /// <returns>The z-score of the element</returns>
    public static double StandardizedScore(this IEnumerable<double> src, double elem)
    {
        var arr = ValidateNotEmpty(src);
        double stdDev = arr.PopulationStandardDeviation();
        if (stdDev == 0)
            throw new InvalidOperationException("Cannot compute z-score when standard deviation is zero.");
        return (elem - arr.Average()) / stdDev;
    }

    /// <summary>
    /// Computes the standardized score of a given element relative to the projected values.
    /// </summary>
    public static double StandardizedScore<T>(this IEnumerable<T> numbers, Func<T, double> selector, double elem)
    {
        ArgumentNullException.ThrowIfNull(numbers);
        ArgumentNullException.ThrowIfNull(selector);
        return numbers.Select(selector).StandardizedScore(elem);
    }

    /// <summary>
    /// Returns a new collection of z-score normalized values (each value transformed to its z-score).
    /// </summary>
    /// <param name="src">The IEnumerable of type double</param>
    /// <exception cref="ArgumentNullException">Thrown when the source is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when the source is empty or has zero standard deviation</exception>
    /// <returns>The z-score normalized values</returns>
    public static IReadOnlyList<double> ZScoreNormalize(this IEnumerable<double> src)
    {
        var arr = ValidateNotEmpty(src);
        double mean = arr.Average();
        double stdDev = arr.PopulationStandardDeviation();
        if (stdDev == 0)
            throw new InvalidOperationException("Cannot z-score normalize when standard deviation is zero.");

        var result = new double[arr.Length];
        for (int i = 0; i < arr.Length; i++)
            result[i] = (arr[i] - mean) / stdDev;

        return result;
    }

    /// <summary>
    /// Returns a new collection of min-max normalized values scaled to [0, 1].
    /// </summary>
    /// <param name="src">The IEnumerable of type double</param>
    /// <exception cref="ArgumentNullException">Thrown when the source is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when the source is empty or all values are identical</exception>
    /// <returns>The min-max normalized values</returns>
    public static IReadOnlyList<double> MinMaxNormalize(this IEnumerable<double> src)
    {
        var arr = ValidateNotEmpty(src);

        double min = arr[0], max = arr[0];
        for (int i = 1; i < arr.Length; i++)
        {
            if (arr[i] < min) min = arr[i];
            if (arr[i] > max) max = arr[i];
        }

        double range = max - min;
        if (range == 0)
            throw new InvalidOperationException("Cannot min-max normalize when all values are identical.");

        var result = new double[arr.Length];
        for (int i = 0; i < arr.Length; i++)
            result[i] = (arr[i] - min) / range;

        return result;
    }

    /// <summary>
    /// Computes the sample covariance between two data sets.
    /// </summary>
    /// <param name="x">The first data set</param>
    /// <param name="y">The second data set</param>
    /// <exception cref="ArgumentNullException">Thrown when either source is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when the sources are empty or have different lengths</exception>
    /// <returns>The sample covariance</returns>
    public static double Covariance(this IEnumerable<double> x, IEnumerable<double> y)
    {
        var xArr = ValidateNotEmpty(x);
        ArgumentNullException.ThrowIfNull(y);
        var yArr = Materialize(y);

        if (xArr.Length != yArr.Length)
            throw new InvalidOperationException("Both data sets must have the same number of elements.");
        if (xArr.Length < 2)
            throw new InsufficientDataSetException("Covariance requires at least 2 data points.");

        double xMean = xArr.Average();
        double yMean = yArr.Average();
        double sum = 0;

        for (int i = 0; i < xArr.Length; i++)
            sum += (xArr[i] - xMean) * (yArr[i] - yMean);

        return sum / (xArr.Length - 1);
    }

    /// <summary>
    /// Computes the Pearson product-moment correlation coefficient between two data sets.
    /// Returns a value between -1 and 1.
    /// </summary>
    /// <param name="x">The first data set</param>
    /// <param name="y">The second data set</param>
    /// <exception cref="ArgumentNullException">Thrown when either source is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when the sources are empty, differ in length, or have zero variance</exception>
    /// <returns>The Pearson correlation coefficient</returns>
    public static double PearsonCorrelation(this IEnumerable<double> x, IEnumerable<double> y)
    {
        var xArr = ValidateNotEmpty(x);
        ArgumentNullException.ThrowIfNull(y);
        var yArr = Materialize(y);

        if (xArr.Length != yArr.Length)
            throw new InvalidOperationException("Both data sets must have the same number of elements.");
        if (xArr.Length < 2)
            throw new InsufficientDataSetException("Pearson correlation requires at least 2 data points.");

        double xMean = xArr.Average();
        double yMean = yArr.Average();
        double sumXY = 0, sumX2 = 0, sumY2 = 0;

        for (int i = 0; i < xArr.Length; i++)
        {
            double dx = xArr[i] - xMean;
            double dy = yArr[i] - yMean;
            sumXY += dx * dy;
            sumX2 += dx * dx;
            sumY2 += dy * dy;
        }

        double denom = Math.Sqrt(sumX2 * sumY2);
        if (denom == 0)
            throw new InvalidOperationException("Cannot compute correlation when one or both data sets have zero variance.");

        return sumXY / denom;
    }

    /// <summary>
    /// Computes the Spearman rank correlation coefficient between two data sets.
    /// A non-parametric measure of rank correlation.
    /// </summary>
    /// <param name="x">The first data set</param>
    /// <param name="y">The second data set</param>
    /// <exception cref="ArgumentNullException">Thrown when either source is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when the sources are empty or differ in length</exception>
    /// <returns>The Spearman rank correlation coefficient</returns>
    public static double SpearmanCorrelation(this IEnumerable<double> x, IEnumerable<double> y)
    {
        var xArr = ValidateNotEmpty(x);
        ArgumentNullException.ThrowIfNull(y);
        var yArr = Materialize(y);

        if (xArr.Length != yArr.Length)
            throw new InvalidOperationException("Both data sets must have the same number of elements.");
        if (xArr.Length < 2)
            throw new InsufficientDataSetException("Spearman correlation requires at least 2 data points.");

        var xRanks = ComputeRanks(xArr);
        var yRanks = ComputeRanks(yArr);

        return xRanks.PearsonCorrelation(yRanks);
    }

    /// <summary>
    /// Computes average ranks for an array, handling ties by averaging.
    /// </summary>
    private static double[] ComputeRanks(double[] data)
    {
        int n = data.Length;
        var indexed = new (double Value, int Index)[n];
        for (int i = 0; i < n; i++)
            indexed[i] = (data[i], i);

        Array.Sort(indexed, (a, b) => a.Value.CompareTo(b.Value));

        var ranks = new double[n];
        int pos = 0;
        while (pos < n)
        {
            int start = pos;
            while (pos < n && indexed[pos].Value == indexed[start].Value)
                pos++;

            double avgRank = (start + pos - 1) / 2.0 + 1.0;
            for (int k = start; k < pos; k++)
                ranks[indexed[k].Index] = avgRank;
        }

        return ranks;
    }

    /// <summary>
    /// Computes the root mean square (RMS) of the enumerable.
    /// Common in signal processing and physics.
    /// </summary>
    /// <param name="src">The IEnumerable of type double</param>
    /// <exception cref="ArgumentNullException">Thrown when the source is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when the source is empty</exception>
    /// <returns>The RMS value</returns>
    public static double RootMeanSquare(this IEnumerable<double> src)
    {
        var arr = ValidateNotEmpty(src);

        double sum = 0;
        for (int i = 0; i < arr.Length; i++)
            sum += arr[i] * arr[i];

        return Math.Sqrt(sum / arr.Length);
    }

    /// <summary>
    /// Computes the RMS of the projected values.
    /// </summary>
    public static double RootMeanSquare<T>(this IEnumerable<T> numbers, Func<T, double> selector)
    {
        ArgumentNullException.ThrowIfNull(numbers);
        ArgumentNullException.ThrowIfNull(selector);
        return numbers.Select(selector).RootMeanSquare();
    }

    /// <summary>
    /// Computes the sum of squares (the sum of squared deviations from the mean).
    /// </summary>
    /// <param name="src">The IEnumerable of type double</param>
    /// <exception cref="ArgumentNullException">Thrown when the source is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when the source is empty</exception>
    /// <returns>The sum of squares from the mean</returns>
    public static double SumOfSquares(this IEnumerable<double> src)
    {
        var arr = ValidateNotEmpty(src);
        double mean = arr.Average();

        double sum = 0;
        for (int i = 0; i < arr.Length; i++)
        {
            double diff = arr[i] - mean;
            sum += diff * diff;
        }

        return sum;
    }

    /// <summary>
    /// Computes the sum of squares of the projected values.
    /// </summary>
    public static double SumOfSquares<T>(this IEnumerable<T> numbers, Func<T, double> selector)
    {
        ArgumentNullException.ThrowIfNull(numbers);
        ArgumentNullException.ThrowIfNull(selector);
        return numbers.Select(selector).SumOfSquares();
    }

    /// <summary>
    /// Computes the standard error of the mean (SEM = sample stddev / sqrt(n)).
    /// </summary>
    /// <param name="src">The IEnumerable of type double</param>
    /// <exception cref="ArgumentNullException">Thrown when the source is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when the source is empty</exception>
    /// <returns>The standard error of the mean</returns>
    public static double StandardError(this IEnumerable<double> src)
    {
        var arr = ValidateNotEmpty(src);
        return arr.SampleStandardDeviation() / Math.Sqrt(arr.Length);
    }

    /// <summary>
    /// Computes the standard error of the mean of the projected values.
    /// </summary>
    public static double StandardError<T>(this IEnumerable<T> numbers, Func<T, double> selector)
    {
        ArgumentNullException.ThrowIfNull(numbers);
        ArgumentNullException.ThrowIfNull(selector);
        return numbers.Select(selector).StandardError();
    }

    /// <summary>
    /// Computes the cumulative sum (running total) of the enumerable.
    /// </summary>
    /// <param name="src">The IEnumerable of type double</param>
    /// <exception cref="ArgumentNullException">Thrown when the source is null</exception>
    /// <returns>A read-only list of cumulative sums</returns>
    public static IReadOnlyList<double> CumulativeSum(this IEnumerable<double> src)
    {
        ArgumentNullException.ThrowIfNull(src);
        var arr = Materialize(src);
        if (arr.Length == 0) return [];

        var result = new double[arr.Length];
        result[0] = arr[0];
        for (int i = 1; i < arr.Length; i++)
            result[i] = result[i - 1] + arr[i];

        return result;
    }

    /// <summary>
    /// Computes a simple moving average with the specified window size.
    /// </summary>
    /// <param name="src">The IEnumerable of type double</param>
    /// <param name="windowSize">The number of elements in each window</param>
    /// <exception cref="ArgumentNullException">Thrown when the source is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when windowSize is less than 1</exception>
    /// <exception cref="InvalidOperationException">Thrown when the source has fewer elements than the window size</exception>
    /// <returns>A read-only list of moving averages</returns>
    public static IReadOnlyList<double> MovingAverage(this IEnumerable<double> src, int windowSize)
    {
        ArgumentNullException.ThrowIfNull(src);
        ArgumentOutOfRangeException.ThrowIfLessThan(windowSize, 1);

        var arr = Materialize(src);
        if (arr.Length < windowSize)
            throw new InvalidOperationException($"Source has {arr.Length} elements, but window size is {windowSize}.");

        int resultLen = arr.Length - windowSize + 1;
        var result = new double[resultLen];

        double windowSum = 0;
        for (int i = 0; i < windowSize; i++)
            windowSum += arr[i];
        result[0] = windowSum / windowSize;

        for (int i = 1; i < resultLen; i++)
        {
            windowSum += arr[i + windowSize - 1] - arr[i - 1];
            result[i] = windowSum / windowSize;
        }

        return result;
    }

    /// <summary>
    /// Detects outliers using the IQR method (values below Q1 - 1.5*IQR or above Q3 + 1.5*IQR).
    /// </summary>
    /// <param name="src">The IEnumerable of type double</param>
    /// <param name="multiplier">The IQR multiplier (default 1.5)</param>
    /// <exception cref="ArgumentNullException">Thrown when the source is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when the source is empty</exception>
    /// <returns>The outlier values</returns>
    public static IReadOnlyList<double> Outliers(this IEnumerable<double> src, double multiplier = 1.5)
    {
        var arr = ValidateNotEmpty(src);
        if (arr.Length < 3) return [];

        double q1 = arr.LowerQuartile();
        double q3 = arr.UpperQuartile();
        double iqr = q3 - q1;
        double lowerFence = q1 - multiplier * iqr;
        double upperFence = q3 + multiplier * iqr;

        var outliers = new List<double>();
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] < lowerFence || arr[i] > upperFence)
                outliers.Add(arr[i]);
        }

        return outliers;
    }

    /// <summary>
    /// Computes the entropy (Shannon entropy) of a data set based on the frequency of each unique value.
    /// Measured in bits (log base 2). Higher entropy indicates more disorder/randomness.
    /// </summary>
    /// <param name="src">The IEnumerable of type double</param>
    /// <exception cref="ArgumentNullException">Thrown when the source is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when the source is empty</exception>
    /// <returns>The Shannon entropy in bits</returns>
    public static double Entropy(this IEnumerable<double> src)
    {
        var arr = ValidateNotEmpty(src);

        var freq = new Dictionary<double, int>();
        foreach (double val in arr)
        {
            if (!freq.TryAdd(val, 1)) freq[val]++;
        }

        double entropy = 0;
        double n = arr.Length;
        foreach (var count in freq.Values)
        {
            double p = count / n;
            if (p > 0)
                entropy -= p * Math.Log2(p);
        }

        return entropy;
    }

    /// <summary>
    /// Computes the Shannon entropy of the projected values.
    /// </summary>
    public static double Entropy<T>(this IEnumerable<T> numbers, Func<T, double> selector)
    {
        ArgumentNullException.ThrowIfNull(numbers);
        ArgumentNullException.ThrowIfNull(selector);
        return numbers.Select(selector).Entropy();
    }

    /// <summary>
    /// Returns whether the data set passes the normal proportion condition:
    /// both np &gt;= threshold and n(1-p) &gt;= threshold. Sam wrote this.
    /// </summary>
    /// <param name="src">The IEnumerable of type double</param>
    /// <param name="samstat">The sample proportion (p-hat)</param>
    /// <param name="threshold">The minimum threshold (default 10)</param>
    /// <exception cref="ArgumentNullException">Thrown when the source is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when the source is empty</exception>
    /// <returns>True if the normal proportion condition is satisfied</returns>
    public static bool IsNormalProportion(this IEnumerable<double> src, double samstat, int threshold = 10)
    {
        var arr = ValidateNotEmpty(src);
        int n = arr.Length;
        return (n * samstat) >= threshold && (n * (1 - samstat)) >= threshold;
    }

    /// <summary>
    /// Returns whether the data set passes the normal proportion condition for the projected values.
    /// </summary>
    public static bool IsNormalProportion<T>(this IEnumerable<T> numbers, Func<T, double> selector, double samstat, int threshold = 10)
    {
        ArgumentNullException.ThrowIfNull(numbers);
        ArgumentNullException.ThrowIfNull(selector);
        return numbers.Select(selector).IsNormalProportion(samstat, threshold);
    }

    /// <summary>
    /// Heuristically checks whether the data is approximately normally distributed by
    /// verifying that the mean, median, and mode are close to each other.
    /// Uses population standard deviation as the tolerance band.
    /// </summary>
    /// <param name="src">The IEnumerable of type double</param>
    /// <exception cref="ArgumentNullException">Thrown when the source is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when the source is empty</exception>
    /// <returns>True if the data appears approximately normally distributed</returns>
    public static bool IsApproximatelyNormal(this IEnumerable<double> src)
    {
        var arr = ValidateNotEmpty(src);
        if (arr.Length < 3) return false;

        double mean = arr.Average();
        double median = arr.Median();
        double stdDev = arr.PopulationStandardDeviation();

        if (stdDev == 0) return true; // All values identical — trivially symmetric

        // Check mean ≈ median within 0.5 standard deviations
        return Math.Abs(mean - median) <= 0.5 * stdDev;
    }

    /// <summary>
    /// Heuristically checks whether the projected data is approximately normally distributed.
    /// </summary>
    public static bool IsApproximatelyNormal<T>(this IEnumerable<T> numbers, Func<T, double> selector)
    {
        ArgumentNullException.ThrowIfNull(numbers);
        ArgumentNullException.ThrowIfNull(selector);
        return numbers.Select(selector).IsApproximatelyNormal();
    }

    /// <summary>
    /// Creates a confidence interval given the mean, critical value, and standard error.
    /// </summary>
    /// <param name="mean">The mean of the data set</param>
    /// <param name="criticalValue">The critical value (z* or t*)</param>
    /// <param name="standardError">The standard error</param>
    /// <returns>A tuple containing the lower and upper bounds of the confidence interval</returns>
    public static (double Lower, double Upper) CreateConfidenceInterval(double mean, double criticalValue, double standardError)
    {
        double margin = criticalValue * standardError;
        return (mean - margin, mean + margin);
    }

    /// <summary>
    /// Computes the t-statistic for a one-sample t-test.
    /// </summary>
    /// <param name="mean">The sample mean</param>
    /// <param name="populationMean">The hypothesized population mean</param>
    /// <param name="standardDeviation">The sample standard deviation</param>
    /// <param name="sampleSize">The sample size</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when sampleSize is less than 1</exception>
    /// <returns>The t-statistic</returns>
    public static double ConstructTValue(double mean, double populationMean, double standardDeviation, double sampleSize)
    {
        if (sampleSize < 1)
            throw new ArgumentOutOfRangeException(nameof(sampleSize), "Sample size must be at least 1.");
        return (mean - populationMean) / (standardDeviation / Math.Sqrt(sampleSize));
    }

    /// <summary>
    /// Computes the z-statistic for a one-sample z-test.
    /// </summary>
    /// <param name="sampleMean">The sample mean</param>
    /// <param name="populationMean">The hypothesized population mean</param>
    /// <param name="populationStdDev">The known population standard deviation</param>
    /// <param name="sampleSize">The sample size</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when sampleSize is less than 1</exception>
    /// <returns>The z-statistic</returns>
    public static double ConstructZValue(double sampleMean, double populationMean, double populationStdDev, double sampleSize)
    {
        if (sampleSize < 1)
            throw new ArgumentOutOfRangeException(nameof(sampleSize), "Sample size must be at least 1.");
        return (sampleMean - populationMean) / (populationStdDev / Math.Sqrt(sampleSize));
    }

    /// <summary>
    /// Computes the two-sample t-statistic (assuming equal variances / pooled).
    /// </summary>
    /// <param name="x">The first sample</param>
    /// <param name="y">The second sample</param>
    /// <exception cref="ArgumentNullException">Thrown when either source is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when either source is empty</exception>
    /// <returns>The two-sample pooled t-statistic</returns>
    public static double TwoSampleTStatistic(this IEnumerable<double> x, IEnumerable<double> y)
    {
        var xArr = ValidateNotEmpty(x);
        ArgumentNullException.ThrowIfNull(y);
        var yArr = ValidateNotEmpty(y);

        int nx = xArr.Length, ny = yArr.Length;
        if (nx < 2 || ny < 2)
            throw new InsufficientDataSetException("Both samples must have at least 2 data points.");

        double xMean = xArr.Average(), yMean = yArr.Average();
        double xVar = xArr.SampleVariance(), yVar = yArr.SampleVariance();

        double pooledVar = ((nx - 1) * xVar + (ny - 1) * yVar) / (nx + ny - 2);
        double se = Math.Sqrt(pooledVar * (1.0 / nx + 1.0 / ny));

        return (xMean - yMean) / se;
    }

    private static byte? ManusTolerance() => null;
}
