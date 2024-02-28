using System.Data.SqlClient;
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

        if (recipeDto.Price < 0)
            throw new ArgumentException("Price must be positive number or 0!");

        if (await repository.CreateAsync(recipeDto) == 0)
            throw new Exception();
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
    }

    public async Task<IdentityUser> GetUserByRecipeIdAsync(int id)
    {
        string userIdQuery = "select UserId from Recipes where Id = @Id";
        string userId = await connection.QueryFirstOrDefaultAsync<string>(userIdQuery, new { Id = id });

        if(userId is null) {
            throw new ArgumentException($"There is no recipe with id: {id}");
        }

        string userQuery = "select * from AspNetUsers where Id = @Id";
        var user = await connection.QueryFirstOrDefaultAsync<IdentityUser>(userQuery, new { Id = userId });

        if (user is null) {
            throw new ArgumentException($"There is no user who has recipe with id: {userId}");
        }

        return user;
    }
}