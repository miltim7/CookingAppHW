public interface IRecipesRepository {
    public Task<IEnumerable<Recipe>> GetAllAsync();
    public Task<int> CreateAsync(RecipeDto recipeDto);
    public Task<Recipe> GetByIdAsync(int id);
}