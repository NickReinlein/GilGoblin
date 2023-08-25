using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Cache;
using GilGoblin.Pocos;
using GilGoblin.Repository;

namespace GilGoblin.Database;

public class RecipeRepository : IRecipeRepository, IRepositoryCache
{
    private readonly GilGoblinDbContext _dbContext;
    private readonly IRecipeCache _recipeCache;
    private readonly IItemRecipeCache _itemRecipesCache;

    public RecipeRepository(
        GilGoblinDbContext recipes,
        IRecipeCache cache,
        IItemRecipeCache itemRecipeCache
    )
    {
        _dbContext = recipes;
        _recipeCache = cache;
        _itemRecipesCache = itemRecipeCache;
    }

    public RecipePoco? Get(int recipeID)
    {
        if (recipeID < 1)
            return null;

        var cached = _recipeCache.Get(recipeID);
        if (cached is not null)
            return cached;

        var recipe = _dbContext?.Recipe?.FirstOrDefault(i => i.ID == recipeID);
        if (recipe is not null)
            _recipeCache.Add(recipe.ID, recipe);

        return recipe;
    }

    public IEnumerable<RecipePoco> GetRecipesForItem(int itemID)
    {
        if (itemID < 1)
            return Enumerable.Empty<RecipePoco>();

        var cached = _itemRecipesCache.Get(itemID);
        if (cached is not null)
            return cached;

        var recipesForItem = _dbContext.Recipe.Where(r => r.TargetItemID == itemID).ToList();
        if (recipesForItem.Any())
            _itemRecipesCache.Add(itemID, recipesForItem);

        return recipesForItem;
    }

    public IEnumerable<RecipePoco?> GetMultiple(IEnumerable<int> recipeIDs) =>
        _dbContext.Recipe.Where(r => recipeIDs.Any(a => a == r.ID));

    public IEnumerable<RecipePoco> GetAll() => _dbContext.Recipe;

    public Task FillCache()
    {
        var recipes = _dbContext.Recipe.ToList();
        recipes.ForEach(recipe => _recipeCache.Add(recipe.ID, recipe));

        var itemIDs = recipes.Select(r => r.TargetItemID).Distinct().ToList();
        itemIDs.ForEach(
            itemID =>
                _itemRecipesCache.Add(
                    itemID,
                    recipes.Where(itemRecipe => itemRecipe.ID == itemID).ToList()
                )
        );
        return Task.CompletedTask;
    }
}
