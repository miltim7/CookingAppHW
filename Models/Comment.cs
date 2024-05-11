public class Comment
{
    public int Id { get; set; }
    public int RecipeId { get; set; }
    public string? AuthorUsername { get; set; }
    public string? Text { get; set; }
    public DateTime? PublishDate { get; set; }
}