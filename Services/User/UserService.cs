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

    public async Task<User> GetUserById(int id)
    {
        string query = "select * from Users where [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<User>(query, new { Id = id });
    }

    public async Task ChangePasswordAsync(string password, int id)
    {
        ArgumentException.ThrowIfNullOrEmpty(password, nameof(password));

        string query = "update Users set [Password] = @Password where Id = @Id;";

        await connection.ExecuteAsync(query, new { Password = password, Id = id });

    }

    public async Task ChangeProfileAsync(User user)
    {
        ArgumentException.ThrowIfNullOrEmpty(user.Name, nameof(user.Name));

        ArgumentException.ThrowIfNullOrEmpty(user.Surname, nameof(user.Surname));

        ArgumentException.ThrowIfNullOrEmpty(user.Login, nameof(user.Login));

        string query = "update Users set [Login] = @Login, [Name] = @Name, [Surname] = @Surname where Id = @Id;";

        await connection.ExecuteAsync(query, user);
    }
}