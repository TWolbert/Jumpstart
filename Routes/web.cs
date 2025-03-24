using PapenChat.Framework;

namespace PapenChat.Routes {
    public class Web {
        public void routes() {
            Router.Get("/", (Request req) => {
                return new Response().JSON(new Dictionary<string, object> {
                    {"message", "Hello from Web!"}
                });
            });
        }
    }
}