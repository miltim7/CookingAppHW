public class RecipeService : IRecipeService
{
    private readonly IRecipesRepository repository;

    public RecipeService(IRecipesRepository repository)
    {
        this.repository = repository;
    }

    public async Task CreateAsync(RecipeDto recipeDto)
    {
        ArgumentException.ThrowIfNullOrEmpty(recipeDto.Title, nameof(recipeDto.Title));

        ArgumentException.ThrowIfNullOrEmpty(recipeDto.Description, nameof(recipeDto.Description));

        ArgumentException.ThrowIfNullOrEmpty(recipeDto.Category, nameof(recipeDto.Category));

        if (recipeDto.Price < 0)
            throw new ArgumentException("Price must be positive number or 0!");
        
        if (await repository.CreateAsync(recipeDto) == 0)
            throw new Exception();
    }
}