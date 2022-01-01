using Ampere.EnumerableUtils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmpereMSTest
{
    [TestClass]
    public class EnumerableUtilsTest
    {
        [TestMethod]
        public void InnerContainsTest_WithIsAllTrue()
        {
            int[] x = { 1, 2, 3, 4, 5, 6, 7 };
            int[] y = { 3, 4, 5 };
            int[] z = { 10, 11, 12 };

            var icp = x.InnerContains(true, y, z);
            Assert.AreEqual(icp.CheckContains(), false);
            Assert.AreEqual(icp.ViolatedEnumerable, z);
        }

        [TestMethod]
        public void InnerContainsTest_WithIsAllFalse()
        {
            int[] x = { 1, 2, 3, 4, 5, 6, 7 };
            int[] y = { 3, 4, 5 };
            int[] z = { 10, 11, 12, 3 };

            var icp = x.InnerContains(false, y, z);
            Assert.AreEqual(icp.CheckContains(), true);
            Assert.AreEqual(icp.ViolatedEnumerable, null);
        }
    }
}
