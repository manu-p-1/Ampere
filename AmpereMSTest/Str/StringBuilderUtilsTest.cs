using Ampere.Str;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
// ReSharper disable StringIndexOfIsCultureSpecific.1
// ReSharper disable ReturnValueOfPureMethodIsNotUsed

namespace AmpereMSTest.Str
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
        public void IndexOf_Test_ExecutionTime()
        {
            var sb = new StringBuilder(File.ReadAllText(@"..\..\..\String\Blob\IpsumLoremParagraph.txt"));
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            sb.IndexOf("Manu");
            stopwatch.Stop();

            var ts = stopwatch.ElapsedMilliseconds;

            Trace.WriteLine($"Elapsed Time is {ts}ms");
        }

        [TestMethod]
        public void IndexOfBuiltIn_Test_ExecutionTime()
        {
            var sb = File.ReadAllText(@"..\..\..\String\Blob\IpsumLoremParagraph.txt");
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            sb.IndexOf("Manu");
            stopwatch.Stop();

            var ts = stopwatch.ElapsedMilliseconds;

            Trace.WriteLine($"Elapsed Time is {ts}ms");
        }

        [TestMethod]
        public void ReplaceOccurrence_Test()
        {
            var sb = new StringBuilder("This is a very very very very long string");
            sb.ReplaceOccurrence("very", "wiki", 2);
            Assert.AreEqual(sb.ToString(), "This is a very wiki very very long string");
        }

        [TestMethod]
        public void ReplaceOccurrence_Test_Numeric()
        {
            var sb = new StringBuilder("1, 2, 1, 1, 3, 4, 5, 6, 1, 1, 4, 5");
            sb.ReplaceOccurrence("1", "54", 4);
            Assert.AreEqual(sb.ToString(), "1, 2, 1, 1, 3, 4, 5, 6, 54, 1, 4, 5");
        }

        [TestMethod]
        public void ReplaceInterval_Test()
        {
            var sb = new StringBuilder("very very very very very very very");
            sb.ReplaceInterval("very", "happy", 2, 3);
            Assert.AreEqual(sb.ToString(), "very happy very very very very very");
        }

        [TestMethod]
        public void AppendIf_Test()
        {
            var sb = new StringBuilder("Manu");
            bool x = true;
            sb.AppendIf(" is Cool", x);
            Assert.AreEqual(sb.ToString(), "Manu is Cool");
        }

        [TestMethod]
        public void AppendIf_StringFalse_NoChange()
        {
            var sb = new StringBuilder("Manu");
            sb.AppendIf(" is Cool", false);
            Assert.AreEqual("Manu", sb.ToString());
        }

        [TestMethod]
        public void AppendLineIf_True_AppendsLine()
        {
            var sb = new StringBuilder("Hello");
            sb.AppendLineIf(" World", true);
            Assert.IsTrue(sb.ToString().StartsWith("Hello World"));
            Assert.IsTrue(sb.ToString().Contains(Environment.NewLine));
        }

        [TestMethod]
        public void AppendLineIf_False_NoChange()
        {
            var sb = new StringBuilder("Hello");
            sb.AppendLineIf(" World", false);
            Assert.AreEqual("Hello", sb.ToString());
        }

        [TestMethod]
        public void AppendJoinFrom_Test()
        {
            var sb = new StringBuilder();
            var items = new[] { 1, 2, 3 };
            sb.AppendJoinFrom(", ", items, i => i.ToString());
            Assert.AreEqual("1, 2, 3", sb.ToString());
        }

        [TestMethod]
        public void AppendJoinFrom_SingleItem()
        {
            var sb = new StringBuilder();
            sb.AppendJoinFrom(", ", new[] { "only" }, s => s);
            Assert.AreEqual("only", sb.ToString());
        }

        [TestMethod]
        public void CountOccurrences_Test()
        {
            var sb = new StringBuilder("hello world hello");
            Assert.AreEqual(2, sb.CountOccurrences("hello"));
        }

        [TestMethod]
        public void CountOccurrences_None()
        {
            var sb = new StringBuilder("hello world");
            Assert.AreEqual(0, sb.CountOccurrences("xyz"));
        }

        [TestMethod]
        public void Contains_Found()
        {
            var sb = new StringBuilder("Hello World");
            Assert.IsTrue(sb.Contains("World"));
        }

        [TestMethod]
        public void Contains_NotFound()
        {
            var sb = new StringBuilder("Hello World");
            Assert.IsFalse(sb.Contains("xyz"));
        }

        [TestMethod]
        public void StartsWith_True()
        {
            var sb = new StringBuilder("Hello World");
            Assert.IsTrue(sb.StartsWith("Hello"));
        }

        [TestMethod]
        public void StartsWith_False()
        {
            var sb = new StringBuilder("Hello World");
            Assert.IsFalse(sb.StartsWith("World"));
        }

        [TestMethod]
        public void EndsWith_True()
        {
            var sb = new StringBuilder("Hello World");
            Assert.IsTrue(sb.EndsWith("World"));
        }

        [TestMethod]
        public void EndsWith_False()
        {
            var sb = new StringBuilder("Hello World");
            Assert.IsFalse(sb.EndsWith("Hello"));
        }

        [TestMethod]
        public void Reverse_Test()
        {
            var sb = new StringBuilder("Hello");
            sb.Reverse();
            Assert.AreEqual("olleH", sb.ToString());
        }

        [TestMethod]
        public void Reverse_SingleChar()
        {
            var sb = new StringBuilder("A");
            sb.Reverse();
            Assert.AreEqual("A", sb.ToString());
        }

        [TestMethod]
        public void TrimStart_Test()
        {
            var sb = new StringBuilder("   Hello");
            sb.TrimStart();
            Assert.AreEqual("Hello", sb.ToString());
        }

        [TestMethod]
        public void TrimEnd_Test()
        {
            var sb = new StringBuilder("Hello   ");
            sb.TrimEnd();
            Assert.AreEqual("Hello", sb.ToString());
        }

        [TestMethod]
        public void Trim_Test()
        {
            var sb = new StringBuilder("   Hello   ");
            sb.Trim();
            Assert.AreEqual("Hello", sb.ToString());
        }

        [TestMethod]
        public void Trim_NoWhitespace_NoChange()
        {
            var sb = new StringBuilder("Hello");
            sb.Trim();
            Assert.AreEqual("Hello", sb.ToString());
        }
    }
}
