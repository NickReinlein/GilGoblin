using GilGoblin.Pocos;

namespace GilGoblin.Repository;

public class RecipeRepository : IRecipeRepository
{
    public RecipeRepository() { }

    public Task<RecipePoco?> Get(int recipeID)
    {
        return new Task<RecipePoco?>(
            () =>
                new RecipePoco
                {
                    RecipeID = recipeID,
                    TargetItemID = recipeID + 1000,
                    IconID = recipeID + 42,
                    ResultQuantity = 1,
                    CanHq = true,
                    CanQuickSynth = true,
                    Ingredients = new List<IngredientPoco>()
                }
        );
    }

    public async Task<IEnumerable<RecipePoco?>> GetAll() => null;

    // Enumerable.Range(1, 5).Select(async (index) => await Get(index)).ToList();

    public async Task<IEnumerable<RecipePoco?>> GetRecipesForItem(int id)
    {
        var recipe = await Get(id);
        return new List<RecipePoco?>() { recipe };
    }
}
