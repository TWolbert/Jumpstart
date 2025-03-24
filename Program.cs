using DotNetEnv;
using PapenChat.Framework;
using PapenChat.Framework.Storage;

namespace PapenChat
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Env.Load();
            StorageController.controller = StorageController.GetStorageController();
            // Listen on 8000 using TCPListener
            var server = new Server("0.0.0.0", 8000);
            server.Start();
        }
    }
}
