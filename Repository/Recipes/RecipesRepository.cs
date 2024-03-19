
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
                        values(@Title, @Description, @Category, @Price, @UserId);
                        select CAST(SCOPE_IDENTITY() as int)";

        int id = await connection.QueryFirstOrDefaultAsync<int>(query, recipeDto);

        return id;
    }

    public async Task<Recipe> GetByIdAsync(int id)
    {
        string query = "select * from Recipes where Id = @Id";

        return await connection.QueryFirstOrDefaultAsync<Recipe>(query, new { Id = id });
    }

    public async Task<int> DeleteAsync(int id)
    {
        string query = "delete from Recipes where Id = @Id";

        return await connection.ExecuteAsync(query, new { Id = id });
    }

    public async Task<int> UpdateAsync(Recipe recipe)
    {
        string query = "update Recipes set Title = @Title, [Description] = @Description, Category = @Category, Price = @Price where Id = @Id";

        return await connection.ExecuteAsync(query, recipe);
    }
}