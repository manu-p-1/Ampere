using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Ampere.AmpFile.FileUtils;
//USE THIS NAMESPACE FOR TESTING METHODS
namespace AmpereMSTest
{
    [TestClass]
    public class FileUtilsTest
    {
        [TestMethod]
        public void GetRootPathTest()
        {
            Assert.AreEqual(System.IO.Path.GetPathRoot(System.Environment.SystemDirectory), GetRootPath());
        }
    }
}
