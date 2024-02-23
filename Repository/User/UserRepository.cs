
using System.Data.SqlClient;
using Dapper;

public class UserRepository : IUserRepository
{
    private readonly SqlConnection connection;
    public UserRepository(SqlConnection connection)
    {
        this.connection = connection; 
    }
    public async Task InsertAsync(UserDto userDto)
    {
        string query = @"insert into Users([Name], [Surname], [Login], [Password])
                        values(@Name, @Surname, @Login, @Password)";
        await connection.ExecuteAsync(query, userDto);
    }

    public async Task<IEnumerable<User>> GetAllAsync() {
        return await connection.QueryAsync<User>("select * from Users");
    }
}