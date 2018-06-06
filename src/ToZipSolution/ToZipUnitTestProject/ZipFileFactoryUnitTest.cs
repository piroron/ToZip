using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.IO;
using ToZip.Services;

namespace ToZipUnitTestProject {
    [TestClass]
    public class ZipFileFactoryUnitTest {
        [TestMethod]
        public void ZipCreateTest() {

            string expectedFile = @"C:\temp\TestDestination\TestSource\TestSource.zip";

            if (File.Exists(expectedFile)) {
                File.Delete(expectedFile);
            }

            var factory = CreateFactory(@"C:\temp\TestSource", @"C:\temp\TestDestination");

            factory.Create(@"C:\temp\TestSource");

            Assert.IsTrue(File.Exists(expectedFile));

            //ZipFileFactory factory = new ZipFileFactory(
        }

        private ZipFileFactory CreateFactory(string source, string dest) {
            return new ZipFileFactory(source, dest);
        }
    }
}
