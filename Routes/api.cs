using PapenChat.Framework;

namespace PapenChat.Routes {
    public class Api {
        public void routes() {
            Router.APIGet("/test", (Request req) => {
                return new Response().JSON(new Dictionary<string, object> {
                    {"message", "Hello from Api!"}
                });
            });

            Router.APIPost("/test", (Request req) => {
                foreach (var key in req.data.Keys) {
                    Console.WriteLine(key + ": " + req.data[key]);
                }
                return new Response().JSON(new Dictionary<string, object> {
                    {"message", req.body}
                });
            });
        }
    }
}