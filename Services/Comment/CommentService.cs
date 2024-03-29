
using System.Data.SqlClient;
using Dapper;

public class CommentService : ICommentService
{
    private readonly ICommentRepository repository;
    private readonly SqlConnection connection;

    public CommentService(ICommentRepository repository, SqlConnection connection)
    {
        this.repository = repository;
        this.connection = connection;
    }
    public async Task CreateAsync(CommentDto dto, string authorUsername)
    {
        ArgumentException.ThrowIfNullOrEmpty(dto.Text, nameof(dto.Text));

        await repository.InsertAsync(new Comment()
        {
            RecipeId = dto.RecipeId,
            AuthorUsername = authorUsername,
            Text = dto.Text,
            PublishDate = DateTime.Now,
        });
    }

    public async Task<IEnumerable<Comment>> GetCommentsByRecipeIdAsync(int id)
    {
        string query = "select * from Comments where RecipeId = @Id";

        return await connection.QueryAsync<Comment>(query, new { Id = id });
    }
}