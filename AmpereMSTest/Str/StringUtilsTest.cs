using Ampere.Str;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
        public void AppendFromEnumerableTest()
        {
            var ss = new List<string>(5) { "How", "Hello", "Are", "You", "Doing" };
            var sb = new StringBuilder(5);
            sb.AppendLineFromEnumerable(ss);
            TestContext.WriteLine(sb.ToString());
        }

        [TestMethod]
        public void Truncate_ShortString_ReturnsOriginal()
        {
            Assert.AreEqual("Hi", "Hi".Truncate(10));
        }

        [TestMethod]
        public void Truncate_LongString_TruncatesWithSuffix()
        {
            Assert.AreEqual("Hello...", "Hello World".Truncate(8));
        }

        [TestMethod]
        public void Truncate_ExactLength_ReturnsOriginal()
        {
            Assert.AreEqual("Hello", "Hello".Truncate(5));
        }

        [TestMethod]
        public void Truncate_CustomSuffix()
        {
            Assert.AreEqual("Hell--", "Hello World".Truncate(6, "--"));
        }

        [TestMethod]
        public void Truncate_ZeroLength_ReturnsEmpty()
        {
            Assert.AreEqual("", "Hello".Truncate(0));
        }

        [TestMethod]
        public void ContainsAny_HasMatch_ReturnsTrue()
        {
            Assert.IsTrue("Hello World".ContainsAny(StringComparison.Ordinal, "xyz", "World"));
        }

        [TestMethod]
        public void ContainsAny_NoMatch_ReturnsFalse()
        {
            Assert.IsFalse("Hello World".ContainsAny(StringComparison.Ordinal, "xyz", "abc"));
        }

        [TestMethod]
        public void ContainsAny_IgnoreCase()
        {
            Assert.IsTrue("Hello World".ContainsAny(StringComparison.OrdinalIgnoreCase, "HELLO"));
        }

        [TestMethod]
        public void ContainsAll_AllPresent_ReturnsTrue()
        {
            Assert.IsTrue("Hello World".ContainsAll(StringComparison.Ordinal, "Hello", "World"));
        }

        [TestMethod]
        public void ContainsAll_MissingOne_ReturnsFalse()
        {
            Assert.IsFalse("Hello World".ContainsAll(StringComparison.Ordinal, "Hello", "xyz"));
        }

        [TestMethod]
        public void Repeat_ThreeTimes()
        {
            Assert.AreEqual("abcabcabc", "abc".Repeat(3));
        }

        [TestMethod]
        public void Repeat_ZeroTimes_ReturnsEmpty()
        {
            Assert.AreEqual("", "abc".Repeat(0));
        }

        [TestMethod]
        public void Repeat_Once_ReturnsOriginal()
        {
            Assert.AreEqual("abc", "abc".Repeat(1));
        }

        [TestMethod]
        public void EqualsIgnoreCase_SameCase_ReturnsTrue()
        {
            Assert.IsTrue("Hello".EqualsIgnoreCase("Hello"));
        }

        [TestMethod]
        public void EqualsIgnoreCase_DifferentCase_ReturnsTrue()
        {
            Assert.IsTrue("Hello".EqualsIgnoreCase("HELLO"));
        }

        [TestMethod]
        public void EqualsIgnoreCase_Different_ReturnsFalse()
        {
            Assert.IsFalse("Hello".EqualsIgnoreCase("World"));
        }

        [TestMethod]
        public void SafeSubstring_WithinBounds()
        {
            Assert.AreEqual("Hel", "Hello".SafeSubstring(0, 3));
        }

        [TestMethod]
        public void SafeSubstring_BeyondLength_ReturnsTruncated()
        {
            Assert.AreEqual("llo", "Hello".SafeSubstring(2, 100));
        }

        [TestMethod]
        public void SafeSubstring_StartBeyondLength_ReturnsEmpty()
        {
            Assert.AreEqual("", "Hello".SafeSubstring(100, 5));
        }

        [TestMethod]
        public void RemoveWhitespace_RemovesAll()
        {
            Assert.AreEqual("HelloWorld", "  Hello  World  ".RemoveWhitespace());
        }

        [TestMethod]
        public void RemoveWhitespace_NoWhitespace_ReturnsOriginal()
        {
            Assert.AreEqual("Hello", "Hello".RemoveWhitespace());
        }

        [TestMethod]
        public void RemoveWhitespace_Empty_ReturnsEmpty()
        {
            Assert.AreEqual("", "".RemoveWhitespace());
        }

        [TestMethod]
        public void ToSnakeCase_PascalCase()
        {
            Assert.AreEqual("hello_world", "HelloWorld".ToSnakeCase());
        }

        [TestMethod]
        public void ToSnakeCase_CamelCase()
        {
            Assert.AreEqual("my_variable_name", "myVariableName".ToSnakeCase());
        }

        [TestMethod]
        public void ToSnakeCase_SingleWord()
        {
            Assert.AreEqual("hello", "Hello".ToSnakeCase());
        }

        [TestMethod]
        public void ToKebabCase_PascalCase()
        {
            Assert.AreEqual("hello-world", "HelloWorld".ToKebabCase());
        }

        [TestMethod]
        public void ToKebabCase_CamelCase()
        {
            Assert.AreEqual("my-variable-name", "myVariableName".ToKebabCase());
        }

        [TestMethod]
        public void ToKebabCase_SingleChar()
        {
            Assert.AreEqual("h", "H".ToKebabCase());
        }
    }
}
