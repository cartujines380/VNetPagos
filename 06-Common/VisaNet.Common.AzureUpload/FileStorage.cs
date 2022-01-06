using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using ExcelDataReader;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.NLog;

namespace VisaNet.Common.AzureUpload
{
    public class FileStorage : IFileStorage
    {
        private static FileStorage _instance;
        private static object _lock = new object();
        private CloudBlobContainer _container;
        private CloudBlobContainer _targetContainer;
        private static string[] _imageFormats = new[] { "png", "jpg", "jpeg", "bmp", "gif", "tiff" };

        private readonly string _imageContainer = ConfigurationManager.AppSettings["AzureImagesContainerName"];
        private readonly string _fileContainer = ConfigurationManager.AppSettings["AzureFilesContainerName"];

        public static FileStorage Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance = new FileStorage();
                    }
                }
                return _instance;
            }
        }

        //Upload
        public void UploadFile(BlobAccessType accessType, string filePath, string contentType)
        {
            var fileName = GetFileName(filePath);
            UploadFile(accessType, filePath, fileName, contentType);
        }

        public void UploadFile(BlobAccessType accessType, string filePath, string filename, string contentType)
        {
            CreateBlob(_fileContainer, accessType);
            var fileStream = File.OpenRead(filePath);

            var blockBlob = _container.GetBlockBlobReference(filename);
            blockBlob.Properties.ContentType = contentType;

            blockBlob.UploadFromStream(fileStream);
        }

        public void UploadFile(BlobAccessType accessType, byte[] file, string folder, string filename, string contentType)
        {
            try
            {
                var fullname = GetFullPath(folder, filename);

                CreateBlob(_fileContainer, accessType);

                var blockBlob = _container.GetBlockBlobReference(fullname);
                blockBlob.Properties.ContentType = contentType;

                blockBlob.UploadFromByteArray(file, 0, file.Length);
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(e);
                throw;
            }
        }

        public void UploadImage(Stream fileStream, string folder, string filename, string contentType)
        {
            var fullname = GetFullPath(folder, filename);

            CreateBlob(_imageContainer, BlobAccessType.Blob);

            var blockBlob = _container.GetBlockBlobReference(fullname);
            blockBlob.Properties.ContentType = contentType;

            blockBlob.UploadFromStream(fileStream);
        }

        //Check if exists
        public bool CheckIfFileExists(string folder, BlobAccessType accessType, string filename)
        {
            var fullname = GetFullPath(folder, filename);

            CreateBlob(_fileContainer, accessType);

            var blockBlob = _container.GetBlockBlobReference(fullname);
            return blockBlob.Exists();
        }

        public bool CheckIfImageExists(string folder, BlobAccessType accessType, string filename)
        {
            var fullname = !string.IsNullOrEmpty(folder) ? string.Format("{0}/{1}", folder, filename) : filename;

            CreateBlob(_imageContainer, accessType);

            var blockBlob = _container.GetBlockBlobReference(fullname);
            return blockBlob.Exists();
        }

        //Delete
        public void DeleteFile(string folder, string filename)
        {
            var fullname = GetFullPath(folder, filename);

            CreateBlob(_fileContainer, BlobAccessType.Blob);
            var blockBlob = _container.GetBlockBlobReference(fullname);
            blockBlob.DeleteIfExists();
        }

        public void DeleteImage(string folder, string filename)
        {
            var fullname = GetFullPath(folder, filename);

            CreateBlob(_imageContainer, BlobAccessType.Blob);
            var blockBlob = _container.GetBlockBlobReference(fullname);
            blockBlob.DeleteIfExists();
        }

        //Get URL
        public string GetUrlFile(string containerName, BlobAccessType accessType, string filename, string[] contentTypes)
        {
            CreateBlob(containerName, accessType);

            var url = string.Empty;

            foreach (var contentType in contentTypes)
            {
                var fullname = string.Format("{0}.{1}", filename, contentType);

                var blockBlob = _container.GetBlockBlobReference(fullname);

                if (blockBlob.Exists())
                {
                    url = blockBlob.Uri.AbsoluteUri;
                    break;
                }
            }
            return url;
        }

        public string GetUrlImageFile(string folder, string filename)
        {
            var fullname = GetFullPath(folder, filename);
            return GetUrlFile(_imageContainer, BlobAccessType.Blob, fullname, _imageFormats);
        }

        public string GetImageUrl(string folder, Guid id, string originImageName, bool tooltip = false)
        {
            string url = null;
            if (!string.IsNullOrEmpty(originImageName))
            {
                if (!string.IsNullOrEmpty(folder) && !folder.EndsWith("/"))
                {
                    folder = folder + "/";
                }
                url = tooltip
                    ? string.Format("{0}{1}_tooltip{2}", folder, id.ToString(), originImageName.Substring(originImageName.LastIndexOf(".")))
                    : string.Format("{0}{1}{2}", folder, id.ToString(), originImageName.Substring(originImageName.LastIndexOf(".")));
            }
            return url;
        }

        public string GetImageTooltipUrl(string folder, Guid id, string originImageName)
        {
            return GetImageUrl(folder, id, originImageName, true);
        }

        //Get file data
        public List<string> GetTextFileLines(string folder, BlobAccessType accessType, string filename)
        {
            var fullname = GetFullPath(folder, filename);
            List<string> result = null;

            CreateBlob(_fileContainer, accessType);
            var blockBlob = _container.GetBlockBlobReference(fullname);

            if (blockBlob.Exists())
            {
                using (var stream = blockBlob.OpenRead())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        result = new List<string>();
                        while (!reader.EndOfStream)
                        {
                            result.Add(reader.ReadLine());
                        }
                    }
                }
            }
            return result;
        }

        public DataSet GetExcelFileData(string folder, BlobAccessType accessType, string filename)
        {
            var fullname = GetFullPath(folder, filename);

            CreateBlob(_fileContainer, accessType);
            var blockBlob = _container.GetBlockBlobReference(fullname);

            DataSet ds;
            using (var memoryStream = new MemoryStream())
            {
                //downloads blob's content to a stream
                blockBlob.DownloadToStream(memoryStream);

                var excelReader = ExcelReaderFactory.CreateOpenXmlReader(memoryStream);
                ds = excelReader.AsDataSet();
                excelReader.Close();
            }

            return ds;
        }

        //Copy
        public void CopyFile(string sourceFolder, string targetFolder, BlobAccessType accessType, string fileName, string newFileName = null)
        {
            var sourceFullpath = GetFullPath(sourceFolder, fileName);
            var targetFullpath = GetFullPath(targetFolder, !string.IsNullOrEmpty(newFileName) ? newFileName : fileName);

            CreateBlob(_fileContainer, accessType);
            CreateTargetBlobForCopy(_fileContainer, accessType);

            var sourceBlob = _container.GetBlockBlobReference(sourceFullpath);
            var targetBlob = _targetContainer.GetBlockBlobReference(targetFullpath);

            targetBlob.StartCopy(sourceBlob);
        }

        public void CopyFileAndDeleteFromSource(string sourceFolder, string targetFolder, BlobAccessType accessType, string fileName, string newFileName = null)
        {
            var sourceFullpath = GetFullPath(sourceFolder, fileName);
            var targetFullpath = GetFullPath(targetFolder, !string.IsNullOrEmpty(newFileName) ? newFileName : fileName);

            CreateBlob(_fileContainer, accessType);
            CreateTargetBlobForCopy(_fileContainer, accessType);

            var sourceBlob = _container.GetBlockBlobReference(sourceFullpath);
            var targetBlob = _targetContainer.GetBlockBlobReference(targetFullpath);

            targetBlob.StartCopy(sourceBlob);
            sourceBlob.DeleteIfExists();
        }

        public void CopyImage(string sourceFolder, string targetFolder, BlobAccessType accessType, string fileName, string newFileName = null)
        {
            var sourceFullpath = GetFullPath(sourceFolder, fileName);
            var targetFullpath = GetFullPath(targetFolder, !string.IsNullOrEmpty(newFileName) ? newFileName : fileName);

            CreateBlob(_imageContainer, accessType);
            CreateTargetBlobForCopy(_imageContainer, accessType);

            var sourceBlob = _container.GetBlockBlobReference(sourceFullpath);
            var targetBlob = _targetContainer.GetBlockBlobReference(targetFullpath);

            targetBlob.StartCopy(sourceBlob);
        }

        public void CopyImageAndDeleteFromSource(string sourceFolder, string targetFolder, BlobAccessType accessType, string fileName, string newFileName = null)
        {
            var sourceFullpath = GetFullPath(sourceFolder, fileName);
            var targetFullpath = GetFullPath(targetFolder, !string.IsNullOrEmpty(newFileName) ? newFileName : fileName);

            CreateBlob(_imageContainer, accessType);
            CreateTargetBlobForCopy(_imageContainer, accessType);

            var sourceBlob = _container.GetBlockBlobReference(sourceFullpath);
            var targetBlob = _targetContainer.GetBlockBlobReference(targetFullpath);

            targetBlob.StartCopy(sourceBlob);
            sourceBlob.DeleteIfExists();
        }

        //Download
        public Stream DownloadFile(string folder, string fileName, BlobAccessType accessType)
        {
            var fullname = GetFullPath(folder, fileName);

            CreateBlob(_fileContainer, accessType);
            var blockBlob = _container.GetBlockBlobReference(fullname);

            Stream blobStream = blockBlob.OpenRead();
            return blobStream;
        }

        //Private methods
        private static string GetFileName(string filePath)
        {
            var pathSpliter = filePath.LastIndexOf(@"\", StringComparison.Ordinal) + 1;
            var fileName = filePath.Substring(pathSpliter);
            return fileName;
        }

        private void CreateBlob(string containerName, BlobAccessType accessType)
        {
            try
            {
                var cloudConnectionString = ConfigurationManager.AppSettings["AzureStorageConnection"];

                // Retrieve storage account from connection string.
                var storageAccount = CloudStorageAccount.Parse(cloudConnectionString);

                // Create the blob client.
                var blobClient = storageAccount.CreateCloudBlobClient();

                // Retrieve reference to a previously created container.
                _container = blobClient.GetContainerReference(containerName);

                if (_container.CreateIfNotExists())
                {
                    // configure container for public access
                    var permissions = _container.GetPermissions();
                    permissions.PublicAccess = GetBlobAccessType(accessType);
                    _container.SetPermissions(permissions);
                }
            }
            catch (Exception ex)
            {
                NLogLogger.LogEvent(ex);
                throw new BusinessException(CodeExceptions.CONNECTION_FAILED);
            }
        }

        private void CreateTargetBlobForCopy(string containerName, BlobAccessType accessType)
        {
            try
            {
                var cloudConnectionString = ConfigurationManager.AppSettings["AzureStorageConnection"];

                // Retrieve storage account from connection string.
                var storageAccount = CloudStorageAccount.Parse(cloudConnectionString);

                // Create the blob client.
                var blobClient = storageAccount.CreateCloudBlobClient();

                // Retrieve reference to a previously created container.
                _targetContainer = blobClient.GetContainerReference(containerName);

                if (_container.CreateIfNotExists())
                {
                    // configure container for public access
                    var permissions = _targetContainer.GetPermissions();
                    permissions.PublicAccess = GetBlobAccessType(accessType);
                    _targetContainer.SetPermissions(permissions);
                }
            }
            catch (Exception ex)
            {
                NLogLogger.LogEvent(ex);
                throw new BusinessException(CodeExceptions.CONNECTION_FAILED);
            }
        }

        private static BlobContainerPublicAccessType GetBlobAccessType(BlobAccessType accessType)
        {
            switch (accessType)
            {
                case BlobAccessType.Private:
                    return BlobContainerPublicAccessType.Off;
                case BlobAccessType.Blob:
                    return BlobContainerPublicAccessType.Blob;
                case BlobAccessType.Container:
                    return BlobContainerPublicAccessType.Container;
                default:
                    throw new ArgumentOutOfRangeException("", accessType, null);
            }
        }

        private string GetFullPath(string folderPath, string fileName)
        {
            if (!string.IsNullOrEmpty(folderPath) && folderPath.EndsWith("/") && folderPath.Length > 1)
            {
                var len = folderPath.Length;
                folderPath = folderPath.Substring(0, len - 1);
            }

            var fullPath = !string.IsNullOrEmpty(folderPath) ? string.Format("{0}/{1}", folderPath, fileName) : fileName;
            return fullPath;
        }

    }
}