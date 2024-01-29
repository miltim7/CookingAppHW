public interface ICookingRepository {
    public Task<IEnumerable<Recipe>> GetAllAsync();
    public Task<int> CreateAsync(RecipeDto recipeDto);
    public Task<Recipe> GetByIdAsync(int id);
}