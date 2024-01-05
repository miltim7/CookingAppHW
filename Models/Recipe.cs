public class Recipe
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public double? Price { get; set; }
    public Recipe(int id, string title, string description, string category, double price)
    {
        this.Id = id;
        this.Title = title;
        this.Description = description;
        this.Category = category;
        this.Price = price;
    }
}