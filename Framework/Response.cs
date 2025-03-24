using System.Text;

namespace PapenChat.Framework
{
    public class Response
    {
        public string body;
        public string status;
        public string contentType;
        public Response(string body = "", string status = "200 OK", string contentType = "text/plain")
        {
            this.body = body;
            this.status = status;
            this.contentType = contentType;
        }

        public Response JSON(Dictionary<string, object> data)
        {
            body = System.Text.Json.JsonSerializer.Serialize(data);
            contentType = "application/json";
            return this;
        }
        public string Serialize()
        {
            byte[] bodyBytes = Encoding.UTF8.GetBytes(body);
            int contentLength = bodyBytes.Length;

            // CORS Headers to be included in the response
            string corsHeaders =
                "Access-Control-Allow-Origin: *\r\n" +  // Allow all origins, adjust as needed
                "Access-Control-Allow-Methods: *\r\n" +  // Allow all HTTP methods (GET, POST, PUT, DELETE, etc.)
                "Access-Control-Allow-Headers: Content-Type, X-Custom-Header\r\n" +  // Allowed headers
                "Access-Control-Allow-Credentials: true\r\n";  // Optional, allows credentials (cookies)

            return $"HTTP/1.1 {status}\r\n" +
                   $"Content-Type: {contentType}\r\n" +
                   $"Content-Length: {contentLength}\r\n" +
                   $"Connection: close\r\n" +
                   corsHeaders +  // Add CORS headers here
                   "\r\n" +  // Blank line to separate headers from body
                   body + "\r\n";  // The body of the response
        }



    }
}