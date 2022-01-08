using System;
using System.Text;
using Ampere.StringUtils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AmpereMSTest
{
    [TestClass]
    public class StringBuilderUtilsTest
    {
        [TestMethod]
        public void IndexOf_Test_NormalCase()
        {
            var sb = new StringBuilder("This is a String");
            Assert.AreEqual(sb.IndexOf("is", 0), 2);
        }

        [TestMethod]
        public void IndexOf_Test_EdgeCase()
        {
            var sb = new StringBuilder("This is a String");
            Assert.AreEqual(sb.IndexOf("g", 0), sb.Length - 1);
        }


        [TestMethod]
        public void IndexOf_Test_NonExistentCase()
        {
            var sb = new StringBuilder("This is a String");
            Assert.AreEqual(sb.IndexOf("x", 0), -1);
        }

        [TestMethod]
        public void IndexOf_Test_EdgeCaseTwo()
        {
            var sb = new StringBuilder("This is a String");
            Assert.AreEqual(sb.IndexOf("a", 0), 8);
        }

        [TestMethod]
        public void IndexOf_Test_EdgeCaseThree()
        {
            var sb = new StringBuilder("This is a String");
            Assert.AreEqual(sb.IndexOf("a", 8), 8);
        }


        [TestMethod]
        public void IndexOf_Test_EdgeCaseFourWithStartIndexAfterValue()
        {
            var sb = new StringBuilder("This is a String");
            Assert.AreEqual(sb.IndexOf("a", 9), -1);
        }

        [TestMethod]
        public void IndexOf_Test_EdgeCaseNoLength()
        {
            var sb = new StringBuilder("");
            Assert.AreEqual(sb.IndexOf("", 0), 0);
        }

        [TestMethod]
        public void IndexOf_Test_EdgeCaseNoLengthTwo()
        {
            var sb = new StringBuilder("");
            Assert.AreEqual(sb.IndexOf("a", 0), -1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void IndexOf_Test_EdgeCaseNoLengthThree()
        {
            var sb = new StringBuilder("");
            sb.IndexOf("a", 2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void IndexOf_Test_EdgeCaseNoLengthFour()
        {
            var sb = new StringBuilder("");
            sb.IndexOf("", 3);
        }

        [TestMethod]
        public void ReplaceNth_Test()
        {
            var sb = new StringBuilder("This is a very very very very long string");
            sb.ReplaceNth("very", "wiki", 2);
            Assert.AreEqual(sb.ToString(), "This is a very wiki very very long string");
        }

        [TestMethod]
        public void ReplaceNth_Test_Numeric()
        {
            var sb = new StringBuilder("1, 2, 1, 1, 3, 4, 5, 6, 1, 1, 4, 5");
            sb.ReplaceNth("1", "54", 4);
            Assert.AreEqual(sb.ToString(), "1, 2, 1, 1, 3, 4, 5, 6, 54, 1, 4, 5");
        }
    }
}
