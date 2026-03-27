using Ampere.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace AmpereMSTest;

[TestClass]
public class IntRangeTest
{
    [TestMethod]
    public void Enumerator_IncludesMinAndMax()
    {
        var range = new IntRange(1, 5);
        var values = new List<int>();
        foreach (var v in range)
            values.Add(v);
        CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5 }, values);
    }

    [TestMethod]
    public void Enumerator_SingleValue()
    {
        var range = new IntRange(3, 3);
        var values = range.Cast<int>().ToList();
        CollectionAssert.AreEqual(new[] { 3 }, values);
    }

    [TestMethod]
    public void Enumerator_Reset_CanIterateAgain()
    {
        var range = new IntRange(1, 3);
        var enumerator = range.GetEnumerator();
        while (enumerator.MoveNext()) { }
        enumerator.Reset();
        var values = new List<int>();
        while (enumerator.MoveNext())
            values.Add((int)enumerator.Current);
        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, values);
    }

    [TestMethod]
    public void MinMax_Properties()
    {
        var range = new IntRange(10, 20);
        Assert.AreEqual(10, range.Minimum);
        Assert.AreEqual(20, range.Maximum);
    }
}
