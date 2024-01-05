using System.Net;
using System.Reflection;

HttpListener listener = new HttpListener();

const int port = 8080;

listener.Prefixes.Add($"http://*:{port}/");

listener.Start();

System.Console.WriteLine("Server Started Successfully!");

while (true)
{
    var context = await listener.GetContextAsync();

    var endpointItems = context.Request.Url?.AbsolutePath?.Split("/",
    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    var controllerType = Assembly.GetExecutingAssembly()
    .GetTypes().
    Where(type => type.BaseType == typeof(BaseController))
    .FirstOrDefault(type => type.Name.ToLower() == $"{endpointItems?[0].ToLower()}controller");

    if (controllerType is null)
    {
        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
        context.Response.Close();
        continue;
    }

    string normalizedRequestHttpMethod = context.Request.HttpMethod.ToLower();

    var controllerMethod = controllerType
    .GetMethods().
    FirstOrDefault(m =>
    {
        return m.GetCustomAttributes()
        .Any(attr =>
        {
            if (attr is HttpAttribute httpAttribute)
            {
                bool isHttpMethodCorrect = httpAttribute.MethodType.Method.ToLower() == normalizedRequestHttpMethod;

                if (isHttpMethodCorrect)
                {
                    if (endpointItems?.Length == 1 && httpAttribute.NormalizedRouting == null)
                        return true;
                    else if (endpointItems?.Length > 1)
                    {
                        if (httpAttribute.NormalizedRouting is null)
                            return false;
                        else
                        {
                            var expectedEndpoint = string.Join('/', endpointItems[1..]).ToLower();
                            var actualEndpoint = httpAttribute.NormalizedRouting;
                            return actualEndpoint == expectedEndpoint;
                        }
                    }
                }
            }
            return false;
        });
    });

    if (controllerMethod is null)
    {
        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
        context.Response.Close();
        continue;
    }

    var controller = (Activator.CreateInstance(controllerType) as BaseController)!;
    controller.Context = context;
    var methodCall = controllerMethod.Invoke(controller, new object[] { });

    if (methodCall is not null && methodCall is Task asyncMethod) {
        await asyncMethod.WaitAsync(CancellationToken.None);
    }

    context.Response.Close();
}