public interface ICommentService
{
    Task CreateAsync(CommentDto dto, string authorUsername);
    Task<IEnumerable<Comment>> GetCommentsByRecipeIdAsync(int id);
}