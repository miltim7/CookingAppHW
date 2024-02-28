using System.Text;
using Microsoft.AspNetCore.Identity;
public class LogMiddleware : IMiddleware
{
    private readonly ILogRepository repository;
    private readonly UserManager<IdentityUser> userManager;
    public LogMiddleware(ILogRepository repository, UserManager<IdentityUser> userManager)
    {
        this.repository = repository;
        this.userManager = userManager;
    }
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var userId = userManager.GetUserId(context.User);

        var requestBody = await GetRequestBody(context);

        var responseBody = await GetResponseBody(context, next);

        var url = $"{context.Request.Path}{context.Request.QueryString}";

        var statusCode = context.Response.StatusCode;

        var methodType = context.Request.Method;

        await repository.InsertAsync(new Log()
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

    private async Task<string> GetResponseBody(HttpContext context, RequestDelegate next)
    {
        string responseBody = string.Empty;

        var originalBodyStream = context.Response.Body;

        using (var newBodyStream = new MemoryStream())
        {
            context.Response.Body = newBodyStream;

            await next(context);

            newBodyStream.Seek(0, SeekOrigin.Begin);

            responseBody = await new StreamReader(newBodyStream).ReadToEndAsync();

            newBodyStream.Seek(0, SeekOrigin.Begin);

            await newBodyStream.CopyToAsync(originalBodyStream);
        }

        return responseBody;
    }
}