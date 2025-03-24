using Newtonsoft.Json;

namespace PapenChat.Services {
    public class ExampleService {
        public string ServiceName = "ExampleService";
        public Dictionary<string, object> execute(Dictionary<string, object> jsonInput) {
            return new Dictionary<string, object> {
                { "status", JsonConvert.SerializeObject(jsonInput) }
            };
        }
    }
}