using PapenChat.Framework;

namespace PapenChat.Controllers {
    public class UserController {
        public string Prefix = "/user";
        public void routes() {
            Router.Get("/user", (Request req) => {
                return new Response().JSON(new Dictionary<string, object> {
                    {"message", "Hello from UserController!"}
                });
            });
        }
    }
}