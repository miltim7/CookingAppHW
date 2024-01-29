
using System.Data.SqlClient;
using Dapper;

public class CookingRepository : ICookingRepository
{
    private readonly SqlConnection connection;
    public CookingRepository(SqlConnection connection)
    {
        this.connection = connection;
    }

    public async Task<IEnumerable<Recipe>> GetAllAsync()
    {
        return await connection.QueryAsync<Recipe>("select * from Recipe");
    }

    public async Task<int> CreateAsync(RecipeDto recipeDto)
    {
        string query = @"insert into Recipe (Title, [Description], Category, Price)
                         values(@Title, @Description, @Category, @Price)";

        return await connection.ExecuteAsync(query, recipeDto);
    }

    public async Task<Recipe> GetByIdAsync(int id) {
        string query = "select * from Recipe where Id = @Id";

        return await connection.QueryFirstOrDefaultAsync<Recipe>(query, new { Id = id });
    }
}