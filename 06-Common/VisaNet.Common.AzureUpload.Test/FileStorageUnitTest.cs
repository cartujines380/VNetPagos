using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VisaNet.Common.AzureUpload.Test
{
    [TestClass]
    public class FileStorageUnitTest
    {
        [TestMethod]
        public void ShouldFailDueUnexistingFile()
        {
            IFileStorage azureStorage = FileStorage.Instance;
            Assert.IsFalse(azureStorage.CheckIfFileExists("pdf", BlobAccessType.Container, "Test.txt"));
        }

        [TestMethod]
        public void ShouldPassExistingFile()
        {
            IFileStorage azureStorage = FileStorage.Instance;
            azureStorage.UploadFile(BlobAccessType.Container, @"C:\Users\rfernandez.HEXACTA\Desktop\New Text Document.txt", "text/plain");
            Assert.IsTrue(azureStorage.CheckIfFileExists("pdf", BlobAccessType.Container, "New Text Document.txt"));
            azureStorage.DeleteFile("pdf", "New Text Document.txt");
        }

        [TestMethod]
        public void ShouldUploadCertificate()
        {
            IFileStorage azureStorage = FileStorage.Instance;
            azureStorage.UploadFile(BlobAccessType.Private, @"C:\Users\rfernandez.HEXACTA\Desktop\New Text Document.txt", "text/plain");
            Assert.IsTrue(azureStorage.CheckIfFileExists("certificates", BlobAccessType.Private, "New Text Document.txt"));
            azureStorage.DeleteFile("certificates", "New Text Document.txt");
        }
    }
}
