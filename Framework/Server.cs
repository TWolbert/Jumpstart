using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace PapenChat.Framework
{
    public class Server
    {
        public string url;
        public string ip;
        public int port;
        public Server(string ip, int port)
        {
            url = ip + $":{port}/";
            this.ip = ip;
            this.port = port;
        }

        public void Start()
        {
            Console.WriteLine("Server started at " + url);
            TcpListener listener = new TcpListener(IPAddress.Parse(ip), port);
            Router.RegisterControllers();
            Router.RegisterRoutes();
            Router.RegisterServices();
            listener.Start();
            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("Client connected");
                Thread thread = new Thread(() => HandleClient(client));
                thread.Start();
            }
        }

        public void HandleClient(TcpClient client)
        {
            // Start measuring request time.
            Stopwatch stopwatch = Stopwatch.StartNew();

            using NetworkStream stream = client.GetStream();
            using StreamReader reader = new StreamReader(stream);
            using StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };

            // Read the request line.
            string? requestLine = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(requestLine))
            {
                Console.WriteLine("Client disconnected");
                return;
            }
            
            // Read headers until an empty line.
            List<string> headers = new List<string>();
            string? headerLine;
            while (!string.IsNullOrEmpty(headerLine = reader.ReadLine()))
            {
                headers.Add(headerLine);
            }

            // Handle preflight request (OPTIONS method)
            if (requestLine.StartsWith("OPTIONS", StringComparison.InvariantCultureIgnoreCase))
            {
                HandlePreflightRequest(writer);
                stopwatch.Stop();
                Console.WriteLine($"Preflight request took {stopwatch.ElapsedMilliseconds}ms");
                return;
            }

            // Determine body length from Content-Length header (if present).
            int contentLength = 0;
            string? contentLengthHeader = headers.FirstOrDefault(h =>
                h.StartsWith("Content-Length:", StringComparison.InvariantCultureIgnoreCase));
            if (contentLengthHeader != null)
            {
                int.TryParse(contentLengthHeader.Substring("Content-Length:".Length).Trim(), out contentLength);
            }

            // Read the request body if available.
            char[] bodyBuffer = new char[contentLength];
            int readLength = contentLength > 0 ? reader.ReadBlock(bodyBuffer, 0, contentLength) : 0;
            string requestBody = new string(bodyBuffer, 0, readLength);

            string response = HandleRequest(requestLine, headers, requestBody);
            writer.Write(response);

            // Stop the stopwatch and log the elapsed time.
            stopwatch.Stop();
            Console.WriteLine($"{requestLine.Replace(" HTTP/1.1", "")} took {stopwatch.ElapsedMilliseconds}ms");
        }

        private void HandlePreflightRequest(StreamWriter writer)
        {
            // Respond to preflight requests (OPTIONS)
            writer.WriteLine("HTTP/1.1 200 OK");
            writer.WriteLine("Access-Control-Allow-Origin: *");
            writer.WriteLine("Access-Control-Allow-Methods: GET, POST, PUT, DELETE, OPTIONS");
            writer.WriteLine("Access-Control-Allow-Headers: Content-Type, Authorization");
            writer.WriteLine("Access-Control-Max-Age: 86400"); // 1 day
            writer.WriteLine("Content-Length: 0");
            writer.WriteLine(""); // Blank line signaling end of headers
        }

        public string HandleRequest(string requestLine, List<string> headers, string requestBody)
        {
            // Extract method and path from the first line of the request.
            string[] requestParts = requestLine.Split(' ');
            if (requestParts.Length < 2)
                return "Invalid Request";

            string method = requestParts[0];
            string path = requestParts[1];

            Request req = new Request(requestLine, headers, requestBody);
            Response res = Router.Route(req, path, method);

            return res.Serialize();
        }
    }
}
