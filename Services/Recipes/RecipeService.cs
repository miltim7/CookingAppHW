
public class RecipeService : IRecipeService
{
    private readonly IRecipesRepository repository;

    public RecipeService(IRecipesRepository repository)
    {
        this.repository = repository;
    }

    public async Task CreateAsync(RecipeDto recipeDto)
    {
        if (string.IsNullOrWhiteSpace(recipeDto.Title))
        {
            throw new ArgumentException("'Title' Can not be empty");
        }

        if (string.IsNullOrWhiteSpace(recipeDto.Description))
        {
            throw new ArgumentException("'Description' Can not be empty");
        }

        if (string.IsNullOrWhiteSpace(recipeDto.Category))
        {
            throw new ArgumentException("'Category' Can not be empty");
        }

        if (recipeDto.Price < 0)
        {
            throw new ArgumentException("Price must be positive number or 0");
        }
        
        if (await repository.CreateAsync(recipeDto) == 0)
        {
            throw new Exception();
        }
    }
}