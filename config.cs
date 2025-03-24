using DotNetEnv;

namespace PapenChat.Configuration
{
    public static class Config
    {
        public static StorageConfig storage = new StorageConfig();
        public static S3Config s3 = new S3Config();
    }

    public class StorageConfig
    {
        public string storageType = Env.GetString("STORAGE_TYPE") ?? "disk";
        public string basePath = Env.GetString("STORAGE_BASE_PATH") ?? "./storage";
    }

    public class S3Config
    {
        public string s3host = Env.GetString("S3_HOST") ?? "localhost";
        public string s3port = Env.GetString("S3_PORT") ?? "9000";
        public string s3accessKey = Env.GetString("S3_ACCESS_KEY") ?? "minioadmin";
        public string s3secretKey = Env.GetString("S3_SECRET_KEY") ?? "minioadmin";
        public string s3bucket = Env.GetString("S3_BUCKET") ?? "papenchat";
    }
}