public interface IRecipeService {
    Task CreateAsync(RecipeDto recipeDto);
    Task<IEnumerable<Recipe>> GetMyAsync(int id);
}