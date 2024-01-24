using System.Net;

public class HomeController : BaseController {
    [HttpGet]
    public async Task HomePageAsync(HttpListenerContext Context) {
        using var writer = new StreamWriter(Context.Response.OutputStream);

        var pageHtml = await File.ReadAllTextAsync("Views/Home.html");
        await writer.WriteAsync(pageHtml);

        Context.Response.StatusCode = (int)HttpStatusCode.OK;
        Context.Response.ContentType = "text.html";
    }
}