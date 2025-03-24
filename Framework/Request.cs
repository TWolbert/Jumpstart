using Newtonsoft.Json;

namespace PapenChat.Framework {
    public class Request {
        public string method;
        public string path;
        public string body;
        // Data field for body
        public Dictionary<string, string> data;
        public Dictionary<string, string> headers;
        public string contentType;
        // Make attributes Dictionary for passing in custom data for context
        public Dictionary<string, object> attributes = new Dictionary<string, object>();
        public Request(string requestLine, List<string> headers, string body) {
            string[] requestLineParts = requestLine.Split(' ');
            method = requestLineParts[0];
            path = requestLineParts[1];

            this.headers = new Dictionary<string, string>();
            foreach (string header in headers) {
                string[] headerParts = header.Split(": ");
                this.headers[headerParts[0]] = headerParts[1];
            }

            contentType = this.headers.TryGetValue("Content-Type", out string? value) ? value : "text/plain";
            this.body = body;
            
            data = ParseBody();
        }

        public Dictionary<string, string> ParseBody() {
            if (contentType == "application/json") {
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(body) ?? new Dictionary<string, string>();
            }
            else {
                return new Dictionary<string, string>();
            }
        }
    }
}