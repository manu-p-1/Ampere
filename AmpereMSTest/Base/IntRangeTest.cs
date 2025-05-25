using Ampere.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace AmpereMSTest.Base
{
    [TestClass]
    public class IntRangeTest
    {
        [TestMethod]
        public void GetEnumerator_IteratesThroughRange()
        {
            var range = new IntRange(1, 5);
            var result = range.ToList();

            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5 }, result);
        }

        [TestMethod]
        public void IsValid_ValidRange_ReturnsTrue()
        {
            var range = new IntRange(1, 5);
            Assert.IsTrue(range.IsValid());
        }

        [TestMethod]
        public void IsValid_InvalidRange_ReturnsFalse()
        {
            var range = new IntRange(5, 1);
            Assert.IsFalse(range.IsValid());
        }
    }
}