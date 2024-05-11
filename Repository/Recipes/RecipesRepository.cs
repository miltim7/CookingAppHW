
using System.Data.SqlClient;
using Dapper;

public class RecipesRepository : IRecipesRepository
{
    private readonly SqlConnection connection;
    public RecipesRepository(SqlConnection connection)
    {
        this.connection = connection;
    }

    public async Task<IEnumerable<Recipe>> GetAllAsync()
    {
        return await connection.QueryAsync<Recipe>("select * from Recipes");
    }

    public async Task<int> CreateAsync(RecipeDto recipeDto)
    {
        string query = @"insert into Recipes ([Title], [Description], [Category], [Price], [UserId])
                         values(@Title, @Description, @Category, @Price, @UserId)";

        return await connection.ExecuteAsync(query, recipeDto);
    }

    public async Task<Recipe> GetByIdAsync(int id) {
        string query = "select * from Recipes where Id = @Id";

        return await connection.QueryFirstAsync<Recipe>(query, new { Id = id });
    }
}