static public class InstrumentRecipe
{
    public static IEnumerable<RecipeDto> ChangeToDto(IEnumerable<Recipe> recipes)
    {
        IEnumerable<RecipeDto> recipesDto = new List<RecipeDto>();

        foreach (var recipe in recipes)
        {
            recipesDto = recipesDto.Append(new RecipeDto()
            {
                Title = recipe.Title,
                Description = recipe.Description,
                Category = recipe.Category,
                Price = recipe.Price,
                UserId = recipe.UserId
            });
        }

        return recipesDto;
    }
}