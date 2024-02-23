public interface IUserService {
    Task<int> LoginAsync(LoginDto loginDto);
    Task CreateAsync(UserDto userDto);
    Task<int> GetIdByLogin(string login);
    Task<User> GetUserById(int id);
    Task ChangePasswordAsync(string password, int id);
    Task ChangeProfileAsync(User user);
}