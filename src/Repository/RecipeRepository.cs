using GilGoblin.Pocos;

namespace GilGoblin.Repository;

public class RecipeRepository : IRecipeRepository
{
    public RecipePoco Get(int id)
    {
        return new RecipePoco
        {
            RecipeID = id,
            TargetItemID = id + 1000,
            IconID = id + 42,
            ResultQuantity = 1,
            CanHq = true,
            CanQuickSynth = true,
            Ingredients = new List<IngredientPoco>()
        };
    }

    public IEnumerable<RecipePoco> GetAll()
    {
        return Enumerable.Range(1, 5).Select(index => Get(index)).ToArray();
    }
}
