using System.Data.SqlClient;
using System.Net;
using System.Text.Json;
using Dapper;

public class CookingController : BaseController
{
    private const string ConnectionString = "Server=localhost;Database=CookingAppDB;User Id=sa;Password=admin;";

    [HttpGet("GetAll")] 
    public async Task GetRecipesAsync(HttpListenerContext context)
    {
        using var writer = new StreamWriter(context.Response.OutputStream);

        using var connection = new SqlConnection(ConnectionString);
        var recipes = await connection.QueryAsync<Recipe>("select * from Recipe");

        var recipesHtml = recipes.GetHtml();
        await writer.WriteLineAsync(recipesHtml);
        context.Response.ContentType = "text/html";

        context.Response.StatusCode = (int)HttpStatusCode.OK;
    }

    [HttpGet("GetById")]
    public async Task GetRecipeByIdAsync(HttpListenerContext context)
    {
        var recipeIdToGetObj = context.Request.QueryString["id"];

        if (recipeIdToGetObj == null || int.TryParse(recipeIdToGetObj, out int recipeIdToGet) == false)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return;
        }

        using var connection = new SqlConnection(ConnectionString);
        var recipe = await connection.QueryFirstOrDefaultAsync<Recipe>(
            sql: "select top 1 * from Recipe where Id = @Id",
            param: new { Id = recipeIdToGet });

        if (recipe is null)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            return;
        }

        using var writer = new StreamWriter(context.Response.OutputStream);
        await writer.WriteLineAsync(JsonSerializer.Serialize(recipe));

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.OK;
    }

    [HttpPost("Create")]
    public async Task PostRecipeAsync(HttpListenerContext context)
    {
        using var reader = new StreamReader(context.Request.InputStream);

        string json = await reader.ReadToEndAsync();

        var newRecipe = JsonSerializer.Deserialize<Recipe>(json);

        if (newRecipe == null || newRecipe.Price == null || string.IsNullOrWhiteSpace(newRecipe.Title) || string.IsNullOrWhiteSpace(newRecipe.Description) || string.IsNullOrWhiteSpace(newRecipe.Category))
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return;
        }

        using var connection = new SqlConnection(ConnectionString);
        var recipes = await connection.ExecuteAsync(
            @"insert into Recipe (Title, [Description], Category, Price)
            values(@Title, @Description, @Category, @Price)",
            param: newRecipe);

        context.Response.StatusCode = (int)HttpStatusCode.Created;
    }

    [HttpDelete]
    public async Task DeleteRecipeAsync(HttpListenerContext context)
    {
        var recipeIdToDeleteObj = context.Request.QueryString["id"];

        if (recipeIdToDeleteObj == null || int.TryParse(recipeIdToDeleteObj, out int recipeIdToDelete) == false)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return;
        }

        using var connection = new SqlConnection(ConnectionString);
        var deletedRowsCount = await connection.ExecuteAsync(
            @"delete Recipe
            where Id = @Id",
            param: new
            {
                Id = recipeIdToDelete,
            });

        if (deletedRowsCount == 0)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            return;
        }

        context.Response.StatusCode = (int)HttpStatusCode.OK;
    }

    [HttpPut]
    public async Task PutRecipeAsync(HttpListenerContext context)
    {
        var recipeIdToUpdateObj = context.Request.QueryString["id"];

        if (recipeIdToUpdateObj == null || int.TryParse(recipeIdToUpdateObj, out int recipeIdToUpdate) == false)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return;
        }

        using var reader = new StreamReader(context.Request.InputStream);
        var json = await reader.ReadToEndAsync();

        var recipeToUpdate = JsonSerializer.Deserialize<Recipe>(json);

        if (recipeToUpdate == null || recipeToUpdate.Price == null || string.IsNullOrWhiteSpace(recipeToUpdate.Title) || string.IsNullOrWhiteSpace(recipeToUpdate.Description) || string.IsNullOrWhiteSpace(recipeToUpdate.Category))
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return;
        }

        using var connection = new SqlConnection(ConnectionString);
        var affectedRowsCount = await connection.ExecuteAsync(
            @"update Recipe
            set Title = @Title, Price = @Price, [Description] = @Description, Category = @Category
            where Id = @Id",
            param: new
            {
                recipeToUpdate.Title,
                recipeToUpdate.Price,
                recipeToUpdate.Description,
                recipeToUpdate.Category,
                Id = recipeIdToUpdate
            });

        if (affectedRowsCount == 0)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            return;
        }

        context.Response.StatusCode = (int)HttpStatusCode.OK;
    }
}