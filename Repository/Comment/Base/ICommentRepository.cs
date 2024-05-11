public interface ICommentRepository
{
    Task InsertAsync(Comment comment);
}