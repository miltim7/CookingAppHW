using System.Data.SqlClient;
using Dapper;

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

    public async Task<IEnumerable<Recipe>> GetMyAsync(int id)
    {
        string query = "select * from Recipes where UserId = @Id";

        return await connection.QueryAsync<Recipe>(query, new { Id = id });
    }
}