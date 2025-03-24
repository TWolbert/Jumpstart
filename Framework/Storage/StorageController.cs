using Minio;
using PapenChat.Framework.Storage.Adapters;
using PapenChat.Configuration;

namespace PapenChat.Framework.Storage
{
    public static class StorageController
    {
        public static GenericStorageController controller;
        public static GenericStorageController GetStorageController()
        {
            return Config.storage.storageType switch
            {
                "disk" => controller ?? new DiskStorageController(),
                "s3" => controller ?? new S3StorageController(),
                _ => throw new Exception("Invalid storage type")
            };
        }
    }

    public class GenericStorageController
    {
        public string basePath = Config.storage.basePath;

        public virtual string SaveFile(string path, string filename, Stream stream)
        {
            var fullPath = GetFullPath(path);
            var fullFilename = $"{fullPath}/{filename}";
            var fullFilePath = Path.Combine(basePath, fullFilename);
            Directory.CreateDirectory(Path.GetDirectoryName(fullFilePath));
            using (var fileStream = File.Create(fullFilePath))
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(fileStream);
            }
            return fullFilename;
        }

        // Get file
        public virtual Stream GetFile(string path)
        {
            var fullPath = GetFullPath(path);
            var fullFilePath = Path.Combine(basePath, fullPath);
            return File.OpenRead(fullFilePath);
        }

        public string GetFullPath(string path)
        {
            return $"{basePath}/{path}";
        }

        public virtual bool DeleteFile(string path)
        {
            var fullPath = GetFullPath(path);
            var fullFilePath = Path.Combine(basePath, fullPath);
            if (File.Exists(fullFilePath))
            {
                File.Delete(fullFilePath);
                return true;
            }
            return false;
        }
    }
}