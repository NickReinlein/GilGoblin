using GilGoblin.Pocos;

namespace GilGoblin.Repository;

public class RecipeRepository : IRecipeRepository
{
    public RecipeRepository() { }

    public RecipePoco? Get(int recipeID) =>
        new()
        {
            RecipeID = recipeID,
            TargetItemID = recipeID + 1000,
            IconID = recipeID + 42,
            ResultQuantity = 1,
            CanHq = true,
            CanQuickSynth = true,
            Ingredients = new List<IngredientPoco>()
        };

    public IEnumerable<RecipePoco?> GetAll() => Enumerable.Range(1, 5).Select(Get).ToList();

    public IEnumerable<RecipePoco?> GetRecipesForItem(int id)
    {
        var recipe = Get(id);
        return new List<RecipePoco?>() { recipe };
    }
}
