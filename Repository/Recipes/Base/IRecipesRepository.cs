public interface IRecipesRepository {
    Task<IEnumerable<Recipe>> GetAllAsync();
    Task<int> CreateAsync(RecipeDto recipeDto);
    Task<Recipe> GetByIdAsync(int id);
    Task<int> DeleteAsync(int id);
    Task<int> UpdateAsync(Recipe recipe);
}