using Ampere.AmpFile;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace AmpereMSTest
{
    [TestClass]
    public class FileUtilsTest
    {
        private readonly string _testFilePath = Path.Combine(Path.GetTempPath(), "testfile.txt");

        [TestInitialize]
        public void Setup()
        {
            if (File.Exists(_testFilePath))
                File.Delete(_testFilePath);
            File.WriteAllText(_testFilePath, "Line1\nLine2\nLine3");
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(_testFilePath))
                File.Delete(_testFilePath);
        }

        [TestMethod]
        public void GetRootPathTest()
        {
            Assert.AreEqual(System.IO.Path.GetPathRoot(System.Environment.SystemDirectory), FileUtils.GetRootPath());
        }

        [TestMethod]
        public void WriteLine_AppendsLineToFile()
        {
            var fileInfo = new FileInfo(_testFilePath);
            FileUtils.WriteLine(fileInfo, "NewLine");

            var lines = File.ReadAllLines(_testFilePath);
            Assert.AreEqual(4, lines.Length);
            Assert.AreEqual("NewLine", lines[^1]);
        }

        [TestMethod]
        public void ReplaceAll_ReplacesTextInFile()
        {
            var fileInfo = new FileInfo(_testFilePath);
            FileUtils.ReplaceAll(fileInfo, "Line2", "ReplacedLine");

            var content = File.ReadAllText(_testFilePath);
            Assert.IsTrue(content.Contains("ReplacedLine"));
            Assert.IsFalse(content.Contains("Line2"));
        }
    }
}
