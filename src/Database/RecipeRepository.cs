using System.Collections.Generic;
using System.Linq;
using GilGoblin.Cache;
using GilGoblin.Pocos;
using GilGoblin.Repository;

namespace GilGoblin.Database;

public class RecipeRepository : IRecipeRepository, IRepositoryCache
{
    private readonly GilGoblinDbContext _dbContext;
    private readonly IRecipeCache _recipeCache;
    private readonly IItemRecipeCache _itemCache;

    public RecipeRepository(
        GilGoblinDbContext recipes,
        IRecipeCache cache,
        IItemRecipeCache itemCache
    )
    {
        _dbContext = recipes;
        _recipeCache = cache;
        _itemCache = itemCache;
    }

    public RecipePoco? Get(int recipeID)
    {
        if (recipeID < 1)
            return null;

        var cached = _recipeCache.Get(recipeID);
        if (cached is not null)
            return cached;

        var item = _dbContext?.Recipe?.FirstOrDefault(i => i.ID == recipeID);
        if (item is not null)
            _recipeCache.Add(item.ID, item);

        return item;
    }

    public IEnumerable<RecipePoco> GetRecipesForItem(int itemID)
    {
        if (itemID < 1)
            return Enumerable.Empty<RecipePoco>();

        var cached = _itemCache.Get(itemID);
        if (cached is not null)
            return cached;

        var recipesForItem = _dbContext.Recipe.Where(r => r.TargetItemID == itemID).ToList();
        if (recipesForItem.Any())
            _itemCache.Add(itemID, recipesForItem);

        return recipesForItem;
    }

    public IEnumerable<RecipePoco?> GetMultiple(IEnumerable<int> recipeIDs) =>
        _dbContext.Recipe.Where(r => recipeIDs.Any(a => a == r.ID));

    public IEnumerable<RecipePoco> GetAll() => _dbContext.Recipe;

    public void FillCache()
    {
        var recipes = _dbContext.Recipe.ToList();
        recipes.ForEach(recipe => _recipeCache.Add(recipe.ID, recipe));

        var itemIDs = recipes.Select(r => r.TargetItemID).Distinct().ToList();
        itemIDs.ForEach(
            itemID =>
                _itemCache.Add(
                    itemID,
                    recipes.Where(itemRecipe => itemRecipe.ID == itemID).ToList()
                )
        );
    }
}
