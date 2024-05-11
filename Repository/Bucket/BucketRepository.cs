using System.Data.SqlClient;
using Dapper;

public class BucketRepository : IBucketRepository
{
    private readonly SqlConnection connection;

    public BucketRepository(SqlConnection connection)
    {
        this.connection = connection;
    }

    public async Task<int> InsertAsync(BucketDto dto)
    {
        string query = @"insert into Bucket ([Title], [Description], [Category], [Price], [UserId])
                        values(@Title, @Description, @Category, @Price, @UserId);";

        return await connection.ExecuteAsync(query, dto);
    }

    public async Task DeleteAsync(int id)
    {
        string query = "delete from Bucket where Id = @Id";

        await connection.ExecuteAsync(query, new { Id = id });
    }

    public async Task<IEnumerable<Bucket>> GetAllAsync()
    {
        string query = "select * from Bucket";

        return await connection.QueryAsync<Bucket>(query);
    }
}