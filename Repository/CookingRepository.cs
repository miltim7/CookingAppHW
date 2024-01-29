
using System.Data.SqlClient;
using Dapper;

public class CookingRepository
{
    private readonly string connectionString;
    public CookingRepository(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("CookingDB");
    }

    public async Task<IEnumerable<Recipe>> GetAll()
    {
        using var connection = new SqlConnection(connectionString);

        string query = "select * from Recipe";
        IEnumerable<Recipe> recipes = await connection.QueryAsync<Recipe>(query);

        return recipes;
    }

    public async Task<bool> Create(RecipeDto recipeDto)
    {
        using var connection = new SqlConnection(connectionString);

        string query = @"insert into Recipe (Title, [Description], Category, Price)
                         values(@Title, @Description, @Category, @Price)";

        if (await connection.ExecuteAsync(query, recipeDto) > 0)
            return true;

        return false;
    }

    public async Task<Recipe> GetById(int id) {
        using var connection = new SqlConnection(connectionString);

        string query = "select * from Recipe where Id = @Id";
        var recipe = await connection.QueryFirstOrDefaultAsync<Recipe>(query, new { Id = id });

        return recipe;
    }
}