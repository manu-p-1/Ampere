using Ampere.Enumerable;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AmpereMSTest
{
    [TestClass]
    public class EnumerableUtilsTest
    {
        [TestMethod]
        public void InnerContainsTest_WithIsAllTrue()
        {
            int[] x = [1, 2, 3, 4, 5, 6, 7];
            int[] y = [3, 4, 5];
            int[] z = [10, 11, 12];

            var icp = x.InnerContains(true, y, z);
            Assert.AreEqual(icp.CheckContains(), false);
            Assert.AreEqual(icp.ViolatedEnumerable, z);
        }

        [TestMethod]
        public void InnerContainsTest_WithIsAllFalse()
        {
            int[] x = [1, 2, 3, 4, 5, 6, 7];
            int[] y = [3, 4, 5];
            int[] z = [10, 11, 12, 3];

            var icp = x.InnerContains(false, y, z);
            Assert.AreEqual(icp.CheckContains(), true);
            Assert.AreEqual(icp.ViolatedEnumerable, null);
        }

        [TestMethod]
        public void InsertTiming()
        {

            var watch2 = Stopwatch.StartNew();
            var y = new[] { 1, 2, 3, 5, 6, 7 }.AsEnumerable();
            y.ToList().Insert(3, 4);
            watch2.Stop();

            Trace.WriteLine("ToList(): " + watch2.Elapsed);

            var watch = Stopwatch.StartNew();
            var x = new[] { 1, 2, 3, 5, 6, 7 };
            EnumerableUtils.Insert(ref x, 3, 1, 4);
            watch.Stop();

            Trace.WriteLine(EnumerableUtils.ToString(x));
            Trace.WriteLine("Enumerable Utils: " + watch.Elapsed);
        }

        [TestMethod]
        public void ToString_FormatsEnumerableCorrectly()
        {
            var list = new List<int> { 1, 2, 3 };
            var result = list.ToString("[|]");
            Assert.AreEqual("[1|2|3]", result);
        }

        [TestMethod]
        public void Insert_AddsElementsToEnumerable()
        {
            var array = new[] { 1, 2, 4 };
            EnumerableUtils.Insert(ref array, 2, 1, 3);

            Assert.AreEqual(4, array.Length);
            Assert.AreEqual(3, array[2]);
        }
    }
}
