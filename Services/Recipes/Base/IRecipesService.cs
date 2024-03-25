using Microsoft.AspNetCore.Identity;

public interface IRecipeService {
    Task CreateAsync(RecipeDto recipeDto);
    Task<IEnumerable<Recipe>> GetMyAsync(string id);
    Task DeleteAsync(int id);
    Task<Recipe> GetById(int id);
    Task EditAsync(Recipe recipe);
    Task<IdentityUser> GetUserByRecipeIdAsync(int id);
}