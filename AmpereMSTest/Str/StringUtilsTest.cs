﻿using System;
using System.Collections.Generic;
using System.Text;
using Ampere.Str;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            var ss = new List<string>(5) { "How", "Hello", "Are", "You","Doing" };
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
        public void ReplaceOverload()
        {
            const string str = "Hello my good good good friend";
            var repl = str.ReplaceRange("good", "nice", 0, str.Length, StringComparison.CurrentCulture);

            Assert.AreEqual("Hello my nice nice nice friend", repl);
            Assert.AreEqual("Hello my good good good friend", str);
        }
    }
}