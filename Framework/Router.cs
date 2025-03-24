using System.Reflection;
using Newtonsoft.Json;

namespace PapenChat.Framework
{
    public class Router
    {
        public enum Method
        {
            GET,
            POST,
            PUT,
            DELETE,
            PATCH,
            OPTIONS,
            HEAD
        }

        // Routes Dict with Method as key and Dictionary<string, Func<Request, Response>> as value
        private static Dictionary<Method, Dictionary<string, Func<Request, Response>>> routes =
            [];

        // Services dict with string as key and Func<Request, Response> as value
        private static Dictionary<string, Func<Dictionary<string, object>, Func<Dictionary<string, object>>>> services =
            [];

        public static void Register(string path, Method method, Func<Request, Response> handler)
        {
            if (!routes.ContainsKey(method))
            {
                routes[method] = [];
            }
            routes[method][path] = handler;
        }

        public static void Get(string path, Func<Request, Response> handler) => Register(path, Method.GET, handler);
        public static void Post(string path, Func<Request, Response> handler) => Register(path, Method.POST, handler);
        public static void Put(string path, Func<Request, Response> handler) => Register(path, Method.PUT, handler);
        public static void Delete(string path, Func<Request, Response> handler) => Register(path, Method.DELETE, handler);
        public static void Patch(string path, Func<Request, Response> handler) => Register(path, Method.PATCH, handler);
        public static void Options(string path, Func<Request, Response> handler) => Register(path, Method.OPTIONS, handler);
        public static void Head(string path, Func<Request, Response> handler) => Register(path, Method.HEAD, handler);

        public static void APIGet(string path, Func<Request, Response> handler) => Register("/api" + path, Method.GET, handler);
        public static void APIPost(string path, Func<Request, Response> handler) => Register("/api" + path, Method.POST, handler);
        public static void APIPut(string path, Func<Request, Response> handler) => Register("/api" + path, Method.PUT, handler);
        public static void APIDelete(string path, Func<Request, Response> handler) => Register("/api" + path, Method.DELETE, handler);
        public static void APIPatch(string path, Func<Request, Response> handler) => Register("/api" + path, Method.PATCH, handler);
        public static void APIOptions(string path, Func<Request, Response> handler) => Register("/api" + path, Method.OPTIONS, handler);
        public static void APIHead(string path, Func<Request, Response> handler) => Register("/api" + path, Method.HEAD, handler);

        public static Response Route(Request request, string path, string methodString)
        {
            Method method = Enum.Parse<Method>(methodString);
            if (path.EndsWith('/'))
            {
                path = path[..^1];
            }
            if (!routes.ContainsKey(method) || !routes[method].ContainsKey(path))
            {
                return new Response("404 Not Found", "404 Not Found", "text/plain");
            }
            return routes[method][path](request);
        }

        internal static void RegisterControllers()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.Namespace == "PapenChat.Controllers")
                {
                    object controller = Activator.CreateInstance(type)!;

                    string prefix = "";
                    FieldInfo? prefixField = type.GetField("Prefix", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (prefixField != null)
                    {
                        prefix = (string)prefixField.GetValue(controller)!;
                    }

                    // Take a snapshot of the currently registered routes per HTTP method
                    var routesBefore = new Dictionary<Method, HashSet<string>>();
                    foreach (Method m in Enum.GetValues(typeof(Method)))
                    {
                        if (routes.ContainsKey(m))
                        {
                            routesBefore[m] = [.. routes[m].Keys];
                        }
                        else
                        {
                            routesBefore[m] = [];
                        }
                    }

                    MethodInfo? method = type.GetMethod("routes");
                    method?.Invoke(controller, null);

                    foreach (Method m in Enum.GetValues(typeof(Method)))
                    {
                        if (routes.ContainsKey(m))
                        {
                            foreach (var key in routes[m].Keys.ToList())
                            {
                                if (!routesBefore[m].Contains(key))
                                {
                                    var handler = routes[m][key];
                                    routes[m].Remove(key);

                                    string newKey = prefix + key;
                                    routes[m][newKey] = handler;
                                }
                            }
                        }
                    }
                }
            }
        }

        internal static void RegisterRoutes()
        {
            // Look in each file in namespace Papenchat.Routes and execute the 'routes(); method if it exists
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.Namespace == "PapenChat.Routes")
                {
                    MethodInfo? method = type.GetMethod("routes");
                    if (method != null)
                    {
                        object routes = Activator.CreateInstance(type)!;
                        method.Invoke(routes, null);
                    }
                }
            }
        }

        internal static void RegisterServices()
        {
            Post("/__service", handleService);

            // Look at each file in namespace Papenchat.Services and as key grab the variable "ServiceName" and as value grab 'execute()' method
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.Namespace == "PapenChat.Services")
                {
                    FieldInfo? serviceNameField = type.GetField("ServiceName");
                    MethodInfo? executeMethod = type.GetMethod("execute");
                    if (serviceNameField != null && executeMethod != null)
                    {
                        object instance = Activator.CreateInstance(type)!;
                        string serviceName = (string)serviceNameField.GetValue(instance)!;
                        services[serviceName] = jsonInput =>
                        {
                            object service = Activator.CreateInstance(type)!;
                            return () => (Dictionary<string, object>)executeMethod.Invoke(service, new object[] { jsonInput })!;
                        };
                    }
                }
            }
        }

        private static Response handleService(Request Req)
        {
            Dictionary<string, object> jsonInput = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(Req.data)) ?? [];
            if (!jsonInput.ContainsKey("service"))
            {
                return new Response("400 Bad Request", "400 Bad Request", "text/plain");
            }
            string serviceName = (string)jsonInput["service"];
            if (!services.ContainsKey(serviceName))
            {
                return new Response("404 Not Found", "404 Not Found", "text/plain");
            }
            Func<Dictionary<string, object>, Func<Dictionary<string, object>>> service = services[serviceName];
            Func<Dictionary<string, object>> result = service(jsonInput);
            var resultData = result();
            return new Response(JsonConvert.SerializeObject(resultData), "200 OK", "application/json");
        }
    }
}