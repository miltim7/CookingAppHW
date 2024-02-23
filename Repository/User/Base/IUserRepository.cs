public interface IUserRepository {
    Task<IEnumerable<User>> GetAllAsync();
    Task InsertAsync(UserDto userDto);
}