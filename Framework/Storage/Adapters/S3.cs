using Minio;
using PapenChat.Configuration;

namespace PapenChat.Framework.Storage.Adapters {
    
    public class S3StorageController : GenericStorageController
    {
        private IMinioClient minio;
        public S3StorageController()
        {
            minio = new MinioClient()
                .WithEndpoint(Config.s3.s3host)
                .WithCredentials(Config.s3.s3accessKey, Config.s3.s3secretKey)
                .WithSSL(true)
                .Build();

            Console.WriteLine($"Storage: S3 storage controller initialized with '{Config.s3.s3host}'");

            if (Config.s3.s3bucket == null)
            {
                throw new Exception("S3_BUCKET is not set");
            }

            var bucketExists = EnsureBucketExists(minio, Config.s3.s3bucket).GetAwaiter().GetResult();
            if (!bucketExists)
            {
                throw new Exception("Bucket does not exist");
            }

            Console.WriteLine($"Storage: S3 bucket '{Config.s3.s3bucket}' exists");

        }

        public async Task<bool> EnsureBucketExists(IMinioClient minio, string bucketName)
        {
            var bucketExists = await minio.BucketExistsAsync(new Minio.DataModel.Args.BucketExistsArgs().WithBucket(bucketName));
            if (!bucketExists)
            {
                return false;
            }
            return true;
        }

        public override string SaveFile(string path, string filename, Stream stream)
        {
            try
            {
                var fullPath = GetFullPath(path);
                var objectName = $"{fullPath}/{filename}";

                minio.PutObjectAsync(new Minio.DataModel.Args.PutObjectArgs()
                    .WithObjectSize(stream.Length)
                    .WithBucket(Config.s3.s3bucket)
                    .WithObject(objectName)
                    .WithStreamData(stream)
                ).GetAwaiter().GetResult();

                return objectName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading to S3: {ex.Message}");
                throw;
            }
        }


        public override Stream GetFile(string path)
        {
            var fullPath = GetFullPath(path);
            var objectName = fullPath;

            var stream = new MemoryStream();

            minio.GetObjectAsync(new Minio.DataModel.Args.GetObjectArgs()
                .WithBucket(Config.s3.s3bucket)
                .WithObject(objectName)
                .WithCallbackStream(remoteStream =>
                {
                    remoteStream.CopyTo(stream);
                })
            ).GetAwaiter().GetResult();
            stream.Position = 0;

            return stream;
        }

        public override bool DeleteFile(string path)
        {
            var fullPath = GetFullPath(path);
            var objectName = fullPath;

            minio.RemoveObjectAsync(new Minio.DataModel.Args.RemoveObjectArgs()
                .WithBucket(Config.s3.s3bucket)
                .WithObject(objectName)
            ).GetAwaiter().GetResult();

            return true;
        }
    }
}