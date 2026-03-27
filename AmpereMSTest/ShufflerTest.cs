using Ampere.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmpereMSTest;

[TestClass]
public class ShufflerTest
{
    // ── Shuffle Array ────────────────────────────────────────

    [TestMethod]
    public void Shuffle_Array_ContainsSameElements()
    {
        int[] arr = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
        var original = (int[])arr.Clone();
        Shuffler.Shuffle(arr);
        CollectionAssert.AreEquivalent(original, arr);
    }

    [TestMethod]
    public void Shuffle_EmptyArray_NoException()
    {
        int[] arr = [];
        Shuffler.Shuffle(arr);
        Assert.AreEqual(0, arr.Length);
    }

    [TestMethod]
    public void Shuffle_SingleElement_Unchanged()
    {
        int[] arr = [42];
        Shuffler.Shuffle(arr);
        Assert.AreEqual(42, arr[0]);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Shuffle_NullArray_Throws()
    {
        Shuffler.Shuffle((int[])null!);
    }

    // ── Shuffle IList ────────────────────────────────────────

    [TestMethod]
    public void Shuffle_IList_ContainsSameElements()
    {
        var list = new List<int> { 1, 2, 3, 4, 5 };
        var original = new List<int>(list);
        Shuffler.Shuffle(list);
        CollectionAssert.AreEquivalent(original, list);
    }

    // ── ShuffleCopy ReadOnlySpan ─────────────────────────────

    [TestMethod]
    public void ShuffleCopy_Span_OriginalUnchanged()
    {
        int[] arr = [1, 2, 3, 4, 5];
        var copy = Shuffler.ShuffleCopy((ReadOnlySpan<int>)arr);
        CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4, 5 }, arr);
        CollectionAssert.AreEquivalent(arr, copy);
    }

    // ── ShuffleCopy IEnumerable ──────────────────────────────

    [TestMethod]
    public void ShuffleCopy_Enumerable_ContainsSameElements()
    {
        IEnumerable<int> src = Enumerable.Range(1, 20);
        var result = Shuffler.ShuffleCopy(src);
        Assert.AreEqual(20, result.Length);
        CollectionAssert.AreEquivalent(Enumerable.Range(1, 20).ToArray(), result);
    }

    // ── PartialShuffle ───────────────────────────────────────

    [TestMethod]
    public void PartialShuffle_ReturnsKElements()
    {
        int[] arr = [10, 20, 30, 40, 50, 60, 70, 80, 90, 100];
        var result = Shuffler.PartialShuffle<int>(arr, 3);
        Assert.AreEqual(3, result.Length);
        foreach (var item in result)
            CollectionAssert.Contains(arr, item);
    }

    [TestMethod]
    public void PartialShuffle_KEqualsN_ReturnsAllElements()
    {
        int[] arr = [1, 2, 3];
        var result = Shuffler.PartialShuffle<int>(arr, 3);
        Assert.AreEqual(3, result.Length);
        CollectionAssert.AreEquivalent(arr, result);
    }

    [TestMethod]
    public void PartialShuffle_KZero_ReturnsEmpty()
    {
        int[] arr = [1, 2, 3];
        var result = Shuffler.PartialShuffle<int>(arr, 0);
        Assert.AreEqual(0, result.Length);
    }

    // ── ReservoirSample ──────────────────────────────────────

    [TestMethod]
    public void ReservoirSample_ReturnsKElements()
    {
        var src = Enumerable.Range(1, 1000);
        var result = Shuffler.ReservoirSample(src, 10);
        Assert.AreEqual(10, result.Length);
        foreach (var item in result)
            Assert.IsTrue(item >= 1 && item <= 1000);
    }

    [TestMethod]
    public void ReservoirSample_KEqualsN_ReturnsAll()
    {
        var src = new[] { 1, 2, 3, 4, 5 };
        var result = Shuffler.ReservoirSample(src, 5);
        Assert.AreEqual(5, result.Length);
        CollectionAssert.AreEquivalent(src, result);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void ReservoirSample_KGreaterThanN_Throws()
    {
        Shuffler.ReservoirSample(new[] { 1, 2 }, 5);
    }

    // ── WeightedSample ───────────────────────────────────────

    [TestMethod]
    public void WeightedSample_ReturnsKElements()
    {
        int[] items = [1, 2, 3, 4, 5];
        double[] weights = [1.0, 2.0, 3.0, 4.0, 5.0];
        var result = Shuffler.WeightedSample<int>(items, weights, 3);
        Assert.AreEqual(3, result.Length);
        foreach (var item in result)
            CollectionAssert.Contains(items, item);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void WeightedSample_MismatchedLengths_Throws()
    {
        Shuffler.WeightedSample<int>(new int[] { 1, 2 }, new double[] { 1.0 }, 1);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void WeightedSample_ZeroWeight_Throws()
    {
        Shuffler.WeightedSample<int>(new int[] { 1, 2 }, new double[] { 1.0, 0.0 }, 1);
    }

    // ── Derange ──────────────────────────────────────────────

    [TestMethod]
    public void Derange_NoFixedPoints()
    {
        int[] arr = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
        // Run multiple times to increase confidence (since Sattolo's is guaranteed, one run suffices)
        var result = Shuffler.Derange<int>(arr);
        Assert.AreEqual(arr.Length, result.Length);
        CollectionAssert.AreEquivalent(arr, result);
        // No element in its original position
        bool hasFixedPoint = false;
        for (int i = 0; i < arr.Length; i++)
            if (result[i] == arr[i])
                hasFixedPoint = true;
        Assert.IsFalse(hasFixedPoint, "Derangement should have no fixed points");
    }

    [TestMethod]
    public void Derange_DoesNotModifyOriginal()
    {
        int[] arr = [1, 2, 3, 4, 5];
        var original = (int[])arr.Clone();
        _ = Shuffler.Derange<int>(arr);
        CollectionAssert.AreEqual(original, arr);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Derange_SingleElement_Throws()
    {
        Shuffler.Derange<int>(new int[] { 1 });
    }
}
