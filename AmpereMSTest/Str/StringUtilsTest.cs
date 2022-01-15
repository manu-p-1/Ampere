using Ampere.Str;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        public void ReplaceOccurrence_Test()
        {
            const string str = "Hello my good good good friend";
            var repl = str.ReplaceOccurrence("good", "very good", 3);

            Assert.AreEqual("Hello my good good very good friend", repl);
        }

        [TestMethod]
        public void ReplaceRange_Test1()
        {
            const string str = "Hello my good good good friend";
            var repl = str.ReplaceRange("good", "nice", 0, str.Length);

            Assert.AreEqual("Hello my nice nice nice friend", repl);
            Assert.AreEqual("Hello my good good good friend", str);
        }

        [TestMethod]
        public void ReplaceRange_Test2()
        {
            const string str = "This is a happy binary tree that is happy";
            var repl = str.ReplaceRange("happy", "excellent", 0);

            Assert.AreEqual("This is a excellent binary tree that is excellent", repl);
        }

        [TestMethod]
        public void ReplaceRange_Test3()
        {
            const string str = "This is a happy binary tree";
            var repl = str.ReplaceRange("happy", "supercalifragilisticexpialidociously amazing", 0);

            Assert.AreEqual("This is a supercalifragilisticexpialidociously amazing binary tree", repl);
        }

        [TestMethod]
        public void ReplaceRange_Test4()
        {
            const string str = "This is a happy binary tree";
            var repl = str.ReplaceRange("china", "japan", 0);

            Assert.AreEqual("This is a happy binary tree", repl);
        }

        [TestMethod]
        public void ReplaceRange_BuiltIn_Test()
        {
            var sb = new StringBuilder(File.ReadAllText(@"..\..\..\Str\Blob\IpsumLoremParagraph.txt"));
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            sb.Replace("ipsum", "manohar", 0, sb.Length);
            stopwatch.Stop();

            var ts = stopwatch.ElapsedMilliseconds;

            Trace.WriteLine($"Replace Range Built-in Elapsed Time is: {ts}ms");
        }

        [TestMethod]
        public void ReplaceRange_Ampere_Test()
        {
            var str = File.ReadAllText(@"..\..\..\Str\Blob\IpsumLoremParagraph.txt");
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            str.ReplaceRange("ipsum", "manohar", 0);
            stopwatch.Stop();

            var ts = stopwatch.ElapsedMilliseconds;

            Trace.WriteLine($"Replace Range Ampere Elapsed Time is: {ts}ms");
        }
    }
}
