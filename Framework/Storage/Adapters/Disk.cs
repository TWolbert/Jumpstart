namespace PapenChat.Framework.Storage.Adapters {
    public class DiskStorageController : GenericStorageController
    {
        public DiskStorageController()
        {
            Console.WriteLine($"Disk storage controller initialized with path {basePath}");
        }

        public override string SaveFile(string path, string filename, Stream stream)
        {
            return base.SaveFile(path, filename, stream);
        }

        public override Stream GetFile(string path)
        {
            return base.GetFile(path);
        }
    }
}