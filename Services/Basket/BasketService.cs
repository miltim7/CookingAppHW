
using System.Data.SqlClient;

public class BucketService : IBucketService
{
    private readonly IBucketRepository repository;

    public BucketService(IBucketRepository repository)
    {
        this.repository = repository;
    }
    public async Task CreateAsync(BucketDto dto)
    {
        ArgumentException.ThrowIfNullOrEmpty(dto.Title, nameof(dto.Title));

        ArgumentException.ThrowIfNullOrEmpty(dto.Description, nameof(dto.Description));

        ArgumentException.ThrowIfNullOrEmpty(dto.Category, nameof(dto.Category));

        if (dto.Price < 0)
            throw new ArgumentException("Price must be positive number or 0!");

        int id = await repository.InsertAsync(dto);
        System.Console.WriteLine(id);
        if (id == 0)
            throw new Exception();
    }

    public async Task DeleteAsync(int id)
    {
        if (id < 1)
            throw new ArgumentException("Id can not be negative nubmer!");

        await repository.DeleteAsync(id);
    }

    public async Task<IEnumerable<Bucket>> GetAllAsync()
    {
        return await repository.GetAllAsync();
    }
}