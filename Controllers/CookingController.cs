using System.Data.SqlClient;
using System.Net;
using System.Text.Json;
using Dapper;

public class CookingController : BaseController
{
    private const string ConnectionString = "Server=localhost;Database=CookingAppDB;User Id=sa;Password=admin;";
    [HttpGet]
    public async Task GetAllAsync()
    {
        using var writer = new StreamWriter(Context.Response.OutputStream);

        using var connection = new SqlConnection(ConnectionString);
        var recipes = await connection.QueryAsync<Recipe>("select * from Recipe");

        var recipesHtml = recipes.GetHtml();
        await writer.WriteLineAsync(recipesHtml);
        Context.Response.ContentType = "text/html";

        Context.Response.StatusCode = (int)HttpStatusCode.OK;
    }
}