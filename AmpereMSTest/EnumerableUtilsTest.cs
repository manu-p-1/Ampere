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
        public void InnerContainsTest()
        {
            int[] x = { 1, 2, 3, 4, 5, 6, 7 };
            int[] y = { 3, 4, 5 };
            int[] z = { 10, 11, 12 };

            var icp = x.InnerContains(y, z);
            Assert.AreEqual(icp.CheckContains(), false);
            Assert.AreEqual(icp.StruckEnumberable, z);
        }
    }
}
