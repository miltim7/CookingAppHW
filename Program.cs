using System.Net;
using System.Reflection;

HttpListener httpListener = new HttpListener();

const int port = 8080;
httpListener.Prefixes.Add($"http://*:{port}/");

httpListener.Start();

while (true)
{
    var context = await httpListener.GetContextAsync();

    var endpointItems = context.Request.Url?.AbsolutePath?.Split("/", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    if (endpointItems == null || endpointItems.Any() == false)
    {
        await new HomeController().HomePageAsync(context);
        context.Response.Close();
        continue;
    }

    var controllerType = Assembly.GetExecutingAssembly()
        .GetTypes()
        .Where(t => t.BaseType == typeof(BaseController))
        .FirstOrDefault(t => t.Name.ToLower() == $"{endpointItems[0]}controller");

    if (controllerType == null)
    {
        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
        context.Response.Close();
        continue;
    }

    string normalizedRequestHttpMethod = context.Request.HttpMethod.ToLower();

    var controllerMethod = controllerType
        .GetMethods()
        .FirstOrDefault(m =>
        {
            return m.GetCustomAttributes()
                .Any(attr =>
                {
                    if (attr is HttpAttribute httpAttribute)
                    {
                        bool isHttpMethodCorrect = httpAttribute.MethodType.Method.ToLower() == normalizedRequestHttpMethod;

                        if (isHttpMethodCorrect)
                        {
                            if (endpointItems.Length == 1 && httpAttribute.NormalizedRouting == null)
                                return true;

                            else if (endpointItems.Length > 1)
                            {
                                if (httpAttribute.NormalizedRouting == null)
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

    if (controllerMethod == null)
    {
        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
        context.Response.Close();
        continue;
    }

    var controller = Activator.CreateInstance(controllerType) as BaseController;
    var methodCall = controllerMethod.Invoke(controller, parameters: new[] { context });

    if (methodCall != null && methodCall is Task asyncMethod)
    {
        await asyncMethod.WaitAsync(CancellationToken.None);
    }

    context.Response.Close();
}