public interface IBucketRepository
{
    Task<int> InsertAsync(BucketDto dto);
    Task DeleteAsync(int id);
    Task<IEnumerable<Bucket>> GetAllAsync();
}