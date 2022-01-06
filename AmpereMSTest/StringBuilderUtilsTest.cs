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
        public void IndexOf_Test_EdgeCaseNoLengthThree()
        {
            var sb = new StringBuilder("");
            Assert.AreEqual(sb.IndexOf("a", 2), -1);
        }

        [TestMethod]
        public void IndexOf_Test_EdgeCaseNoLengthFour()
        {
            var sb = new StringBuilder("");
            Assert.AreEqual(sb.IndexOf("", 3), -1);
        }

        [TestMethod]
        public void ReplaceAll_Test()
        {
            var sb = new StringBuilder("This is a very long string");
            sb.ReplaceAll("very", "replaced");
            Assert.AreEqual(sb.ToString(), "This is a replaced long string");
        }

        [TestMethod]
        public void ReplaceAll_Test_Numeric()
        {
            var sb = new StringBuilder("1, 2, 1, 1, 3, 4, 5, 6, 1, 1, 4, 5");
            sb.ReplaceAll("1", "54");
            Assert.AreEqual(sb.ToString(), "54, 2, 54, 54, 3, 4, 5, 6, 54, 54, 4, 5");
        }
    }
}
