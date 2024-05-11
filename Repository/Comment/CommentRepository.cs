
using System.Data.SqlClient;
using Dapper;

public class CommentRepository : ICommentRepository
{
    private readonly SqlConnection connection;

    public CommentRepository(SqlConnection connection)
    {
        this.connection = connection;
    }
    public async Task InsertAsync(Comment comment)
    {
        string query = @"insert into Comments([RecipeId], [AuthorUsername], [Text], [PublishDate]) values (@RecipeId, @AuthorUsername, @Text, @PublishDate)";

        await connection.ExecuteAsync(query, comment);
    }
}