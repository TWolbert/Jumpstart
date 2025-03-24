namespace PapenChat.Utils {
    public class FileUtils {
        public static string GetFileExtension(string filename) {
            return filename.Substring(filename.LastIndexOf('.'));
        }

        // Function to turn base64 string into a byte array
        public static byte[] Base64ToByteArray(string base64) {
            return Convert.FromBase64String(base64);
        }
    }
}