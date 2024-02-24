using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Api.Cache;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;

namespace GilGoblin.Api.Repository;

public class RecipeRepository : IRecipeRepository
{
    private readonly GilGoblinDbContext _dbContext;
    private readonly IRecipeCache _recipeCache;
    private readonly IItemRecipeCache _itemRecipesCache;

    public RecipeRepository(
        GilGoblinDbContext dbContext,
        IRecipeCache cache,
        IItemRecipeCache itemRecipeCache
    )
    {
        _dbContext = dbContext;
        _recipeCache = cache;
        _itemRecipesCache = itemRecipeCache;
    }

    public RecipePoco? Get(int itemId)
    {
        if (itemId < 1)
            return null;

        try
        {
            var cached = _recipeCache.Get(itemId);
            if (cached is not null)
                return cached;

            var recipe = _dbContext.Recipe.FirstOrDefault(i => i.Id == itemId);
            if (recipe is not null)
                _recipeCache.Add(recipe.Id, recipe);

            return recipe;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public IEnumerable<RecipePoco> GetRecipesForItem(int itemId)
    {
        if (itemId < 1)
            return Enumerable.Empty<RecipePoco>();

        var cached = _itemRecipesCache.Get(itemId);
        if (cached is not null)
            return cached;

        var recipesForItem = _dbContext.Recipe.Where(r => r.TargetItemId == itemId).ToList();
        if (recipesForItem.Any())
            _itemRecipesCache.Add(itemId, recipesForItem);

        return recipesForItem;
    }

    public IEnumerable<RecipePoco?> GetMultiple(IEnumerable<int> itemIds) =>
        _dbContext.Recipe.Where(r => itemIds.Any(a => a == r.Id)).AsEnumerable();

    public IEnumerable<RecipePoco> GetAll() =>
        _dbContext.Recipe.AsEnumerable();

    public Task FillCache()
    {
        var recipes = _dbContext.Recipe.ToList();
        recipes.ForEach(recipe => _recipeCache.Add(recipe.Id, recipe));

        var ids = recipes.Select(r => r.TargetItemId).Distinct().ToList();
        ids.ForEach(
            itemId =>
                _itemRecipesCache.Add(
                    itemId,
                    recipes.Where(itemRecipe => itemRecipe.Id == itemId).ToList()
                )
        );
        return Task.CompletedTask;
    }
}