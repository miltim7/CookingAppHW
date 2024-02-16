using System.Data.SqlClient;
using Dapper;

public class UserService : IUserService
{
    private readonly SqlConnection connection;
    private readonly IUserRepository repository;

    public UserService(SqlConnection connection, IUserRepository repository)
    {
        this.connection = connection;
        this.repository = repository;
    }
    public async Task CreateAsync(UserDto userDto)
    {
        ArgumentException.ThrowIfNullOrEmpty(userDto.Name, nameof(userDto.Name));

        ArgumentException.ThrowIfNullOrEmpty(userDto.Surname, nameof(userDto.Surname));

        ArgumentException.ThrowIfNullOrEmpty(userDto.Login, nameof(userDto.Login));

        ArgumentException.ThrowIfNullOrEmpty(userDto.Password, nameof(userDto.Password));

        await repository.InsertAsync(userDto);
    }

    public async Task<int> LoginAsync(LoginDto loginDto)
    {
        ArgumentException.ThrowIfNullOrEmpty(loginDto.Login, nameof(loginDto.Login));

        ArgumentException.ThrowIfNullOrEmpty(loginDto.Password, nameof(loginDto.Password));

        string userQuery = "select * from Users where [Login] = @Login and [Password] = @Password";
        var user = await connection.QueryFirstOrDefaultAsync<User>(userQuery, loginDto);

        if (user is null)
        {
            string iUserExistQuery = "select * from Users where Login = @Login";
            var isLoginExist = await connection.QueryFirstOrDefaultAsync<User>(iUserExistQuery, loginDto) is not null;

            if (isLoginExist)
                throw new ArgumentException("Password Is Wrong!");
            else
                throw new ArgumentException($"User {loginDto.Login} does not exist!");
        }

        return user.Id;
    }

    public async Task<int> GetIdByLogin(string login)
    {
        string query = "select * from Users where [Login] = @Login";

        return (await connection.QueryFirstOrDefaultAsync<User>(query, new { Login = login })).Id;
    }
}