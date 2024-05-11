public interface IBucketService
{
    Task CreateAsync(BucketDto basket);
    Task DeleteAsync(int id);
    Task<IEnumerable<Bucket>> GetAllAsync();
}