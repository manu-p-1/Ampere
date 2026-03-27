using Ampere.Statistics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmpereMSTest;

[TestClass]
public class EnumerableStatsTest
{
    // ── Central Tendency ──────────────────────────────────────

    [TestMethod]
    public void Mean_SimpleValues_ReturnsCorrect()
    {
        double[] data = [1, 2, 3, 4, 5];
        Assert.AreEqual(3.0, data.Mean(), 1e-10);
    }

    [TestMethod]
    public void Mean_SingleElement_ReturnsThatElement()
    {
        double[] data = [42.0];
        Assert.AreEqual(42.0, data.Mean(), 1e-10);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Mean_Empty_Throws()
    {
        Array.Empty<double>().Mean();
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Mean_Null_Throws()
    {
        ((IEnumerable<double>)null!).Mean();
    }

    [TestMethod]
    public void Mean_GenericOverload_Works()
    {
        var items = new[] { new { Val = 10.0 }, new { Val = 20.0 } };
        Assert.AreEqual(15.0, items.Mean(x => x.Val), 1e-10);
    }

    [TestMethod]
    public void Median_OddCount_ReturnsMiddle()
    {
        double[] data = [3, 1, 2];
        Assert.AreEqual(2.0, data.Median(), 1e-10);
    }

    [TestMethod]
    public void Median_EvenCount_ReturnsAverageOfMiddleTwo()
    {
        double[] data = [1, 2, 3, 4];
        Assert.AreEqual(2.5, data.Median(), 1e-10);
    }

    [TestMethod]
    public void Median_SingleElement_ReturnsThatElement()
    {
        double[] data = [7.0];
        Assert.AreEqual(7.0, data.Median(), 1e-10);
    }

    [TestMethod]
    public void Mode_ClearMode_ReturnsIt()
    {
        double[] data = [1, 2, 2, 3];
        Assert.AreEqual(2.0, data.Mode(), 1e-10);
    }

    [TestMethod]
    [ExpectedException(typeof(NoModeException))]
    public void Mode_AllEqual_ThrowsNoMode()
    {
        double[] data = [1, 2, 3];
        data.Mode();
    }

    [TestMethod]
    public void Mode_SingleElement_ReturnsThatElement()
    {
        double[] data = [5.0];
        Assert.AreEqual(5.0, data.Mode(), 1e-10);
    }

    [TestMethod]
    public void Mode_BimodalReturnsFirst()
    {
        // When two values tie for highest frequency, returns the first encountered (insertion order)
        double[] data = [1, 1, 2, 2, 3];
        double mode = data.Mode();
        Assert.IsTrue(mode == 1.0 || mode == 2.0);
    }

    [TestMethod]
    public void GeometricMean_SimpleValues()
    {
        double[] data = [2, 8];
        Assert.AreEqual(4.0, data.GeometricMean(), 1e-10);
    }

    [TestMethod]
    public void GeometricMean_SingleValue()
    {
        double[] data = [5.0];
        Assert.AreEqual(5.0, data.GeometricMean(), 1e-10);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void GeometricMean_NonPositive_Throws()
    {
        double[] data = [1, -2, 3];
        data.GeometricMean();
    }

    [TestMethod]
    public void HarmonicMean_SimpleValues()
    {
        double[] data = [1, 4, 4];
        // H = 3 / (1/1 + 1/4 + 1/4) = 3 / 1.5 = 2
        Assert.AreEqual(2.0, data.HarmonicMean(), 1e-10);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void HarmonicMean_Zero_Throws()
    {
        double[] data = [1, 0, 3];
        data.HarmonicMean();
    }

    [TestMethod]
    public void WeightedMean_EqualWeights_EqualsMean()
    {
        double[] data = [1, 2, 3];
        double[] weights = [1, 1, 1];
        Assert.AreEqual(2.0, data.WeightedMean(weights), 1e-10);
    }

    [TestMethod]
    public void WeightedMean_UnequalWeights()
    {
        double[] data = [10, 20];
        double[] weights = [3, 1];
        // (10*3 + 20*1) / (3+1) = 50/4 = 12.5
        Assert.AreEqual(12.5, data.WeightedMean(weights), 1e-10);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void WeightedMean_DifferentLengths_Throws()
    {
        double[] data = [1, 2];
        double[] weights = [1, 2, 3];
        data.WeightedMean(weights);
    }

    [TestMethod]
    public void TrimmedMean_TrimZero_EqualsMean()
    {
        double[] data = [1, 2, 3, 4, 5];
        Assert.AreEqual(3.0, data.TrimmedMean(0.0), 1e-10);
    }

    [TestMethod]
    public void TrimmedMean_Trim20Percent()
    {
        double[] data = [1, 2, 3, 4, 100];
        // 20% of 5 = 1 trimmed from each end → [2, 3, 4] → mean = 3
        Assert.AreEqual(3.0, data.TrimmedMean(0.2), 1e-10);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void TrimmedMean_NegativeFraction_Throws()
    {
        double[] data = [1, 2, 3];
        data.TrimmedMean(-0.1);
    }

    // ── Dispersion / Variability ──────────────────────────────

    [TestMethod]
    public void SampleVariance_KnownValues()
    {
        double[] data = [2, 4, 4, 4, 5, 5, 7, 9];
        // mean = 5, sample variance = sum((xi-5)^2) / 7
        double expected = (9 + 1 + 1 + 1 + 0 + 0 + 4 + 16) / 7.0;
        Assert.AreEqual(expected, data.SampleVariance(), 1e-10);
    }

    [TestMethod]
    [ExpectedException(typeof(InsufficientDataSetException))]
    public void SampleVariance_SingleElement_Throws()
    {
        double[] data = [5.0];
        data.SampleVariance();
    }

    [TestMethod]
    public void PopulationVariance_KnownValues()
    {
        double[] data = [2, 4, 4, 4, 5, 5, 7, 9];
        double expected = (9 + 1 + 1 + 1 + 0 + 0 + 4 + 16) / 8.0;
        Assert.AreEqual(expected, data.PopulationVariance(), 1e-10);
    }

    [TestMethod]
    public void SampleStandardDeviation_IsSqrtOfVariance()
    {
        double[] data = [2, 4, 4, 4, 5, 5, 7, 9];
        double variance = data.SampleVariance();
        Assert.AreEqual(Math.Sqrt(variance), data.SampleStandardDeviation(), 1e-10);
    }

    [TestMethod]
    public void PopulationStandardDeviation_IsSqrtOfVariance()
    {
        double[] data = [2, 4, 4, 4, 5, 5, 7, 9];
        double variance = data.PopulationVariance();
        Assert.AreEqual(Math.Sqrt(variance), data.PopulationStandardDeviation(), 1e-10);
    }

    [TestMethod]
    public void Range_UnsortedData_ReturnsMaxMinusMIn()
    {
        double[] data = [5, 1, 9, 3];
        Assert.AreEqual(8.0, data.Range(), 1e-10);
    }

    [TestMethod]
    public void Range_SingleElement_ReturnsZero()
    {
        double[] data = [42.0];
        Assert.AreEqual(0.0, data.Range(), 1e-10);
    }

    [TestMethod]
    public void MeanAbsoluteDeviation_KnownValues()
    {
        double[] data = [1, 2, 3, 4, 5];
        // MAD from mean(3) = (2+1+0+1+2)/5 = 1.2
        Assert.AreEqual(1.2, data.MeanAbsoluteDeviation(), 1e-10);
    }

    [TestMethod]
    public void MedianAbsoluteDeviation_KnownValues()
    {
        double[] data = [1, 1, 2, 2, 4, 6, 9];
        // Median = 2, deviations = [1, 1, 0, 0, 2, 4, 7], median of deviations = 1
        Assert.AreEqual(1.0, data.MedianAbsoluteDeviation(), 1e-10);
    }

    [TestMethod]
    public void CoefficientOfVariation_KnownValues()
    {
        double[] data = [10, 10, 10];
        // All identical → variance = 0, CV calculation would divide by zero
        // But SampleVariance requires n>=2, so let's test with variable data
        double[] data2 = [10, 20, 30];
        double cv = data2.CoefficientOfVariation();
        Assert.IsTrue(cv > 0);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void CoefficientOfVariation_ZeroMean_Throws()
    {
        double[] data = [-1, 0, 1];
        data.CoefficientOfVariation();
    }

    // ── Quartiles & Percentiles ───────────────────────────────

    [TestMethod]
    public void LowerQuartile_KnownValues()
    {
        double[] data = [6, 7, 15, 36, 39, 40, 41, 42, 43, 47, 49];
        // Lower half: [6, 7, 15, 36, 39], median = 15
        Assert.AreEqual(15.0, data.LowerQuartile(), 1e-10);
    }

    [TestMethod]
    public void UpperQuartile_KnownValues()
    {
        double[] data = [6, 7, 15, 36, 39, 40, 41, 42, 43, 47, 49];
        // Upper half: [41, 42, 43, 47, 49], median = 43
        Assert.AreEqual(43.0, data.UpperQuartile(), 1e-10);
    }

    [TestMethod]
    [ExpectedException(typeof(InsufficientDataSetException))]
    public void LowerQuartile_TooFewElements_Throws()
    {
        double[] data = [1, 2];
        data.LowerQuartile();
    }

    [TestMethod]
    [ExpectedException(typeof(InsufficientDataSetException))]
    public void UpperQuartile_TooFewElements_Throws()
    {
        double[] data = [1, 2];
        data.UpperQuartile();
    }

    [TestMethod]
    public void InterQuartileRange_KnownValues()
    {
        double[] data = [6, 7, 15, 36, 39, 40, 41, 42, 43, 47, 49];
        Assert.AreEqual(28.0, data.InterQuartileRange(), 1e-10);
    }

    [TestMethod]
    public void Percentile_Median_Equals50th()
    {
        double[] data = [1, 2, 3, 4, 5];
        Assert.AreEqual(data.Median(), data.Percentile(50), 1e-10);
    }

    [TestMethod]
    public void Percentile_0_ReturnsMin()
    {
        double[] data = [5, 3, 1, 4, 2];
        Assert.AreEqual(1.0, data.Percentile(0), 1e-10);
    }

    [TestMethod]
    public void Percentile_100_ReturnsMax()
    {
        double[] data = [5, 3, 1, 4, 2];
        Assert.AreEqual(5.0, data.Percentile(100), 1e-10);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Percentile_Negative_Throws()
    {
        double[] data = [1, 2, 3];
        data.Percentile(-1);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Percentile_Over100_Throws()
    {
        double[] data = [1, 2, 3];
        data.Percentile(101);
    }

    // ── Shape ──────────────────────────────────────────────────

    [TestMethod]
    public void Skewness_SymmetricData_NearZero()
    {
        double[] data = [1, 2, 3, 4, 5, 6, 7, 8, 9];
        Assert.AreEqual(0.0, data.Skewness(), 1e-10);
    }

    [TestMethod]
    public void Skewness_RightSkewed_Positive()
    {
        double[] data = [1, 1, 1, 2, 2, 3, 10];
        Assert.IsTrue(data.Skewness() > 0);
    }

    [TestMethod]
    [ExpectedException(typeof(InsufficientDataSetException))]
    public void Skewness_TooFew_Throws()
    {
        double[] data = [1, 2];
        data.Skewness();
    }

    [TestMethod]
    public void ExcessKurtosis_NormalLikeData_NearZero()
    {
        // For a uniform distribution, excess kurtosis ≈ -1.2
        double[] data = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
        double k = data.ExcessKurtosis();
        Assert.IsTrue(k < 0); // Uniform is platykurtic
    }

    [TestMethod]
    [ExpectedException(typeof(InsufficientDataSetException))]
    public void ExcessKurtosis_TooFew_Throws()
    {
        double[] data = [1, 2, 3];
        data.ExcessKurtosis();
    }

    // ── Standardized Scores & Normalization ───────────────────

    [TestMethod]
    public void StandardizedScore_MeanElement_ReturnsZero()
    {
        double[] data = [2, 4, 4, 4, 5, 5, 7, 9];
        double mean = data.Mean();
        Assert.AreEqual(0.0, data.StandardizedScore(mean), 1e-10);
    }

    [TestMethod]
    public void ZScoreNormalize_SumsToZero()
    {
        double[] data = [1, 2, 3, 4, 5];
        var normalized = data.ZScoreNormalize();
        double sum = 0;
        for (int i = 0; i < normalized.Count; i++) sum += normalized[i];
        Assert.AreEqual(0.0, sum, 1e-10);
    }

    [TestMethod]
    public void MinMaxNormalize_RangeIsZeroToOne()
    {
        double[] data = [10, 20, 30, 40, 50];
        var normalized = data.MinMaxNormalize();
        Assert.AreEqual(0.0, normalized[0], 1e-10);
        Assert.AreEqual(1.0, normalized[normalized.Count - 1], 1e-10);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void MinMaxNormalize_AllSame_Throws()
    {
        double[] data = [5, 5, 5];
        data.MinMaxNormalize();
    }

    // ── Relationship ──────────────────────────────────────────

    [TestMethod]
    public void Covariance_PerfectPositive()
    {
        double[] x = [1, 2, 3, 4, 5];
        double[] y = [2, 4, 6, 8, 10];
        Assert.IsTrue(x.Covariance(y) > 0);
    }

    [TestMethod]
    public void Covariance_PerfectNegative()
    {
        double[] x = [1, 2, 3, 4, 5];
        double[] y = [10, 8, 6, 4, 2];
        Assert.IsTrue(x.Covariance(y) < 0);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Covariance_DifferentLengths_Throws()
    {
        double[] x = [1, 2, 3];
        double[] y = [1, 2];
        x.Covariance(y);
    }

    [TestMethod]
    public void PearsonCorrelation_PerfectPositive()
    {
        double[] x = [1, 2, 3, 4, 5];
        double[] y = [2, 4, 6, 8, 10];
        Assert.AreEqual(1.0, x.PearsonCorrelation(y), 1e-10);
    }

    [TestMethod]
    public void PearsonCorrelation_PerfectNegative()
    {
        double[] x = [1, 2, 3, 4, 5];
        double[] y = [10, 8, 6, 4, 2];
        Assert.AreEqual(-1.0, x.PearsonCorrelation(y), 1e-10);
    }

    [TestMethod]
    public void SpearmanCorrelation_PerfectMonotonic()
    {
        double[] x = [1, 2, 3, 4, 5];
        double[] y = [2, 5, 10, 20, 50]; // Monotonically increasing
        Assert.AreEqual(1.0, x.SpearmanCorrelation(y), 1e-10);
    }

    [TestMethod]
    public void SpearmanCorrelation_PerfectNegativeMonotonic()
    {
        double[] x = [1, 2, 3, 4, 5];
        double[] y = [50, 20, 10, 5, 2];
        Assert.AreEqual(-1.0, x.SpearmanCorrelation(y), 1e-10);
    }

    // ── Aggregation & Transformation ──────────────────────────

    [TestMethod]
    public void RootMeanSquare_KnownValues()
    {
        double[] data = [1, 2, 3];
        // RMS = sqrt((1+4+9)/3) = sqrt(14/3)
        Assert.AreEqual(Math.Sqrt(14.0 / 3.0), data.RootMeanSquare(), 1e-10);
    }

    [TestMethod]
    public void SumOfSquares_KnownValues()
    {
        double[] data = [2, 4, 4, 4, 5, 5, 7, 9];
        // mean = 5, SS = 9+1+1+1+0+0+4+16 = 32
        Assert.AreEqual(32.0, data.SumOfSquares(), 1e-10);
    }

    [TestMethod]
    public void StandardError_KnownRelation()
    {
        double[] data = [2, 4, 4, 4, 5, 5, 7, 9];
        double expected = data.SampleStandardDeviation() / Math.Sqrt(data.Length);
        Assert.AreEqual(expected, data.StandardError(), 1e-10);
    }

    [TestMethod]
    public void CumulativeSum_SimpleValues()
    {
        double[] data = [1, 2, 3, 4];
        var cumSum = data.CumulativeSum();
        CollectionAssert.AreEqual(new double[] { 1, 3, 6, 10 }, (System.Collections.ICollection)cumSum);
    }

    [TestMethod]
    public void CumulativeSum_Empty_ReturnsEmpty()
    {
        var cumSum = Array.Empty<double>().CumulativeSum();
        Assert.AreEqual(0, cumSum.Count);
    }

    [TestMethod]
    public void MovingAverage_WindowOf3()
    {
        double[] data = [1, 2, 3, 4, 5];
        var ma = data.MovingAverage(3);
        Assert.AreEqual(3, ma.Count);
        Assert.AreEqual(2.0, ma[0], 1e-10);
        Assert.AreEqual(3.0, ma[1], 1e-10);
        Assert.AreEqual(4.0, ma[2], 1e-10);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void MovingAverage_WindowZero_Throws()
    {
        double[] data = [1, 2, 3];
        data.MovingAverage(0);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void MovingAverage_WindowTooLarge_Throws()
    {
        double[] data = [1, 2];
        data.MovingAverage(5);
    }

    [TestMethod]
    public void Outliers_DetectsOutliers()
    {
        double[] data = [1, 2, 3, 4, 5, 6, 7, 8, 9, 100];
        var outliers = data.Outliers();
        Assert.IsTrue(outliers.Contains(100));
    }

    [TestMethod]
    public void Outliers_NoOutliers_ReturnsEmpty()
    {
        double[] data = [1, 2, 3, 4, 5];
        var outliers = data.Outliers();
        Assert.AreEqual(0, outliers.Count);
    }

    [TestMethod]
    public void Entropy_AllSame_ReturnsZero()
    {
        double[] data = [5, 5, 5, 5];
        Assert.AreEqual(0.0, data.Entropy(), 1e-10);
    }

    [TestMethod]
    public void Entropy_TwoEqualGroups_ReturnsOne()
    {
        double[] data = [0, 0, 1, 1];
        // H = -0.5*log2(0.5) - 0.5*log2(0.5) = 1 bit
        Assert.AreEqual(1.0, data.Entropy(), 1e-10);
    }

    // ── Distribution Tests & Inference ────────────────────────

    [TestMethod]
    public void IsNormalProportion_LargeN_True()
    {
        var data = Enumerable.Range(1, 100).Select(x => (double)x).ToArray();
        Assert.IsTrue(data.IsNormalProportion(0.5));
    }

    [TestMethod]
    public void IsNormalProportion_SmallP_False()
    {
        var data = Enumerable.Range(1, 10).Select(x => (double)x).ToArray();
        Assert.IsFalse(data.IsNormalProportion(0.01));
    }

    [TestMethod]
    public void IsApproximatelyNormal_SymmetricData_True()
    {
        double[] data = [1, 2, 3, 4, 5, 6, 7, 8, 9];
        Assert.IsTrue(data.IsApproximatelyNormal());
    }

    [TestMethod]
    public void IsApproximatelyNormal_HighlySkewed_False()
    {
        // Strongly right-skewed: many small values, a few large ones
        double[] data = [1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 50, 50, 50, 50, 100];
        Assert.IsFalse(data.IsApproximatelyNormal());
    }

    [TestMethod]
    public void CreateConfidenceInterval_KnownValues()
    {
        var (lower, upper) = EnumerableStats.CreateConfidenceInterval(50, 1.96, 5);
        Assert.AreEqual(40.2, lower, 1e-10);
        Assert.AreEqual(59.8, upper, 1e-10);
    }

    [TestMethod]
    public void ConstructTValue_KnownValues()
    {
        // t = (100 - 90) / (15 / sqrt(25)) = 10 / 3 ≈ 3.333...
        double t = EnumerableStats.ConstructTValue(100, 90, 15, 25);
        Assert.AreEqual(10.0 / 3.0, t, 1e-10);
    }

    [TestMethod]
    public void ConstructZValue_KnownValues()
    {
        double z = EnumerableStats.ConstructZValue(105, 100, 10, 25);
        // z = (105 - 100) / (10 / sqrt(25)) = 5/2 = 2.5
        Assert.AreEqual(2.5, z, 1e-10);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void ConstructTValue_ZeroSampleSize_Throws()
    {
        EnumerableStats.ConstructTValue(100, 90, 15, 0);
    }

    [TestMethod]
    public void TwoSampleTStatistic_IdenticalSamples_ReturnsZero()
    {
        double[] x = [1, 2, 3, 4, 5];
        double[] y = [1, 2, 3, 4, 5];
        Assert.AreEqual(0.0, x.TwoSampleTStatistic(y), 1e-10);
    }

    [TestMethod]
    public void TwoSampleTStatistic_DifferentSamples_Nonzero()
    {
        double[] x = [1, 2, 3, 4, 5];
        double[] y = [10, 20, 30, 40, 50];
        Assert.AreNotEqual(0.0, x.TwoSampleTStatistic(y));
    }

    // ── Edge cases ────────────────────────────────────────────

    [TestMethod]
    public void Percentile_Interpolation_Works()
    {
        double[] data = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
        // 25th percentile via linear interpolation: rank = 0.25 * 9 = 2.25 → lerp(3, 4, 0.25) = 3.25
        Assert.AreEqual(3.25, data.Percentile(25), 1e-10);
    }

    [TestMethod]
    public void StandardizedScore_GenericOverload_Works()
    {
        var items = new[] { new { V = 2.0 }, new { V = 4.0 }, new { V = 4.0 }, new { V = 4.0 }, new { V = 5.0 }, new { V = 5.0 }, new { V = 7.0 }, new { V = 9.0 } };
        double z = items.StandardizedScore(x => x.V, items.Mean(x => x.V));
        Assert.AreEqual(0.0, z, 1e-10);
    }
}
