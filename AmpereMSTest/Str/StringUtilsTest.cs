using Ampere.Str;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text;

namespace AmpereMSTest.Str
{
    [TestClass]
    public class StringUtilsTest
    {
        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void IsWellFormedTest()
        {
            var dt = new Dictionary<char, char>
            {
                ['<'] = '>',
                ['('] = ')'
            };

            Assert.AreEqual(true, "<<((Manu))>>".IsWellFormed(dt));
            Assert.AreEqual(false, "<<((Manu)>>".IsWellFormed(dt));
        }

        [TestMethod]
        public void AppendFromEnumerableTest()
        {
            var ss = new List<string>(5) { "How", "Hello", "Are", "You", "Doing" };
            var sb = new StringBuilder(5);
            sb.AppendLineFromEnumerable(ss);
            TestContext.WriteLine(sb.ToString());
        }

        [TestMethod]
        public void IsSystemDateTime_ValidDate_ReturnsTrue()
        {
            var result = "2025-05-25".IsSystemDateTime("yyyy-MM-dd");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsSystemDateTime_InvalidDate_ReturnsFalse()
        {
            var result = "invalid-date".IsSystemDateTime("yyyy-MM-dd");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ContainsDigits_StringWithDigits_ReturnsTrue()
        {
            var result = "abc123".ContainsDigits();
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ContainsDigits_StringWithoutDigits_ReturnsFalse()
        {
            var result = "abcdef".ContainsDigits();
            Assert.IsFalse(result);
        }
    }
}
