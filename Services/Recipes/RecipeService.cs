using System.Data.SqlClient;
using System.Net;
using Dapper;
using Microsoft.AspNetCore.Identity;

public class RecipeService : IRecipeService
{
    private readonly SqlConnection connection;
    private readonly IRecipesRepository repository;

    public RecipeService(SqlConnection connection, IRecipesRepository repository)
    {
        this.connection = connection;
        this.repository = repository;
    }

    public async Task CreateAsync(RecipeDto recipeDto)
    {
        ArgumentException.ThrowIfNullOrEmpty(recipeDto.Title, nameof(recipeDto.Title));

        ArgumentException.ThrowIfNullOrEmpty(recipeDto.Description, nameof(recipeDto.Description));

        ArgumentException.ThrowIfNullOrEmpty(recipeDto.Category, nameof(recipeDto.Category));

        var recipe = await SearchByTitleAsync(recipeDto.Title);

        if (recipe != null)
            throw new ArgumentException($"'{recipeDto.Title}' Title is already used");

        if (recipeDto.Price < 0)
            throw new ArgumentException("Price must be positive number or 0!");

        if (await repository.CreateAsync(recipeDto) == 0)
            throw new Exception();

        await DownloadImage(recipeDto.Title);
    }

    private async Task DownloadImage(string title)
    {
        string url = $"https://source.unsplash.com/featured/?{title}";
        string path = "wwwroot/images/recipes";

        int id = (await SearchByTitleAsync(title)).Id;

        string fileName = $"{id}.png";

        using WebClient client = new WebClient();

        string headerValue = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3";

        client.Headers.Add("user-agent", headerValue);

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        string localPath = Path.Combine(path, fileName);

        client.DownloadFile(url, localPath);
    }

    private async Task<Recipe> SearchByTitleAsync(string title)
    {
        string query = "select * from Recipes where Title = @Title";
        var recipe = await connection.QueryFirstOrDefaultAsync<Recipe>(query, new { Title = title });
        return recipe;
    }
    public async Task<IEnumerable<Recipe>> GetMyAsync(string id)
    {
        string query = "select * from Recipes where UserId = @Id";

        return await connection.QueryAsync<Recipe>(query, new { Id = id });
    }

    public async Task DeleteAsync(int id)
    {
        if (id < 1)
        {
            throw new ArgumentException("Id can't be negative nubmer!");
        }

        if (await repository.DeleteAsync(id) == 0)
        {
            throw new Exception("There is no recipe with that Id!");
        }

        DeleteImage(id);
    }

    public async Task<Recipe> GetById(int id)
    {
        string query = "select * from Recipes where Id = @Id";

        return await connection.QueryFirstOrDefaultAsync<Recipe>(query, new { Id = id });
    }

    public async Task Edit(Recipe recipe)
    {
        ArgumentException.ThrowIfNullOrEmpty(recipe.Title, nameof(recipe.Title));

        ArgumentException.ThrowIfNullOrEmpty(recipe.Description, nameof(recipe.Description));

        ArgumentException.ThrowIfNullOrEmpty(recipe.Category, nameof(recipe.Category));

        if (recipe.Price < 0)
        {
            throw new ArgumentException("Price can't be negative nubmer!");
        }

        if (await repository.UpdateAsync(recipe) == 0)
        {
            throw new Exception();
        }

        await UpdateImage(recipe);
    }

    private async Task UpdateImage(Recipe recipe)
    {
        DeleteImage(recipe.Id);      

        await DownloadImage(recipe.Title);
    }

    private void DeleteImage(int id)
    {
        string fileNameToDelete = $"{id}.png";

        string path = "wwwroot/images/recipes";

        string filePath = Path.Combine(path, fileNameToDelete);

        File.Delete(filePath);
    }

    public async Task<IdentityUser> GetUserByRecipeIdAsync(int id)
    {
        string userIdQuery = "select UserId from Recipes where Id = @Id";
        string userId = await connection.QueryFirstOrDefaultAsync<string>(userIdQuery, new { Id = id });

        if (userId is null)
        {
            throw new ArgumentException($"There is no recipe with id: {id}");
        }

        string userQuery = "select * from AspNetUsers where Id = @Id";
        var user = await connection.QueryFirstOrDefaultAsync<IdentityUser>(userQuery, new { Id = userId });

        if (user is null)
        {
            throw new ArgumentException($"There is no user who has recipe with id: {userId}");
        }

        return user;
    }
}