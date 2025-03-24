using PapenChat.Framework;
using PapenChat.Framework.Storage;
using PapenChat.Models;
using PapenChat.Utils;

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

            Router.APIPost("/image", (Request req) => {
                string image = req.data["profilePicture"].Split(",")[1];

                var imageDataArr = FileUtils.Base64ToByteArray(image);

                StorageController.controller.SaveFile("pfp", "test.png", new MemoryStream(imageDataArr));

                return new Response().JSON(new Dictionary<string, object> {
                    {"message", "Hello from Api!"}
                });
            });
        }
    }
}