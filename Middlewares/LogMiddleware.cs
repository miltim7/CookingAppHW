
using System.Text;

public class LogMiddleware : IMiddleware
{
    private readonly ILogService service;
    public LogMiddleware(ILogService service)
    {
        this.service = service;
    }
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        string userId;
        if (context.Request.Cookies["UserId"] is null)
        {
            userId = "unauthorized";
        }
        else
        {
            userId = context.Request.Cookies["UserId"];
        }


        string requestBody = await GetRequestBody(context);

        await next.Invoke(context);

        string responseBody = await GetResponseBody(context);


        string url = $"{context.Request.Path}{context.Request.QueryString}";

        int statusCode = context.Response.StatusCode;

        string methodType = context.Request.Method;

        await service.Log(new Log()
        {
            UserId = userId,
            Url = url,
            MethodType = methodType,
            StatusCode = statusCode,
            RequestBody = requestBody,
            ResponseBody = responseBody
        });
    }

    private async Task<string> GetRequestBody(HttpContext context)
    {
        if (!context.Request.Body.CanSeek)
            context.Request.EnableBuffering();
        context.Request.Body.Position = 0;
        StreamReader requestReader = new(context.Request.Body, Encoding.UTF8);
        string requestBody = await requestReader.ReadToEndAsync();
        context.Request.Body.Position = 0;

        return requestBody;
    }

    private async Task<string> GetResponseBody(HttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        string responseBody = string.Empty;
        using (StreamReader reader = new StreamReader(context.Response.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: true))
        {
            responseBody = await reader.ReadToEndAsync();
            context.Response.Body.Position = 0; // Сбросить позицию потока обратно на начало
        }

        return responseBody;
    }
}