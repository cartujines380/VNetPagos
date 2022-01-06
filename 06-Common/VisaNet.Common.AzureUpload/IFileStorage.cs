using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace VisaNet.Common.AzureUpload
{
    public interface IFileStorage
    {
        //Upload
        /// <summary>
        /// Uploads an existing file
        /// </summary>
        /// <param name="filePath">Path to the file</param>
        /// <param name="contentType">Content Type</param>
        void UploadFile(BlobAccessType accessType, string filePath, string contentType);

        /// <summary>
        /// Uploads a file renaming it
        /// </summary>
        /// <param name="filePath">Path to the file</param>
        /// <param name="filename">New file name</param>
        /// <param name="contentType">Content Type</param>
        void UploadFile(BlobAccessType accessType, string filePath, string filename, string contentType);

        void UploadFile(BlobAccessType accessType, byte[] file, string folder, string filename, string contentType);

        void UploadImage(Stream fileStream, string folder, string filename, string contentType);

        //Check if exists
        /// <summary>
        /// Checks if a file exists matching the given filename
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        bool CheckIfFileExists(string folder, BlobAccessType accessType, string filename);

        bool CheckIfImageExists(string folder, BlobAccessType accessType, string filename);

        //Delete
        /// <summary>
        /// Deletes the file that matches the name
        /// </summary>
        /// <param name="filename"></param>
        void DeleteFile(string folder, string filename);

        void DeleteImage(string folder, string filename);

        //Get URL
        /// <summary>
        /// Get URI of filename if exists
        /// </summary>
        /// <param name="filename"></param>
        string GetUrlFile(string containerName, BlobAccessType accessType, string filename, string[] contentTypes);

        string GetUrlImageFile(string folder, string filename);

        string GetImageUrl(string folder, Guid id, string originImageName, bool tooltip = false);

        string GetImageTooltipUrl(string folder, Guid id, string originImageName);

        //Get file data
        List<string> GetTextFileLines(string folder, BlobAccessType accessType, string filename);

        DataSet GetExcelFileData(string folder, BlobAccessType accessType, string filename);

        //Copy
        void CopyFile(string sourceFolder, string targetFolder, BlobAccessType accessType, string fileName, string newFileName = null);

        void CopyFileAndDeleteFromSource(string sourceFolder, string targetFolder, BlobAccessType accessType, string fileName, string newFileName = null);

        void CopyImage(string sourceFolder, string targetFolder, BlobAccessType accessType, string fileName, string newFileName = null);

        void CopyImageAndDeleteFromSource(string sourceFolder, string targetFolder, BlobAccessType accessType, string fileName, string newFileName = null);

    }
}