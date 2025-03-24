using PapenChat.Framework;
using PapenChat.Models;

namespace PapenChat.Routes {
    public class Api {
        public void routes() {
            Router.APIGet("/test", (Request req) => {
                return new Response().JSON(new Dictionary<string, object> {
                    {"message", "Hello from Api!"}
                });
            });

            Router.APIPost("/test", (Request req) => {
                Task<Users> userTask = Users.FindAsync(1);

                var user = userTask.Result;

                return new Response().JSON(new Dictionary<string, object> {
                    {"message", user}
                });
            });
        }
    }
}