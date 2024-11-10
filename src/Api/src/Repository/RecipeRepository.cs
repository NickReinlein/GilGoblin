using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Api.Cache;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;
using Microsoft.Extensions.DependencyInjection;

namespace GilGoblin.Api.Repository;

public interface IRecipeRepository : IDataRepository<RecipePoco>
{
    List<RecipePoco> GetRecipesForItem(int itemId);
    List<RecipePoco> GetRecipesForItemIds(IEnumerable<int> itemIds);
}

public class RecipeRepository(
    IServiceProvider serviceProvider,
    IRecipeCache recipeCache,
    IItemRecipeCache itemRecipeCache)
    : IRecipeRepository
{
    public RecipePoco? Get(int itemId)
    {
        if (itemId < 1)
            return null;

        try
        {
            var cached = recipeCache.Get(itemId);
            if (cached is not null)
                return cached;

            using var scope = serviceProvider.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
            var recipe = dbContext.Recipe.FirstOrDefault(i => i.Id == itemId);
            if (recipe is not null)
                recipeCache.Add(recipe.Id, recipe);
            return recipe;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public List<RecipePoco> GetRecipesForItem(int itemId)
    {
        if (itemId < 1)
            return [];

        var cached = itemRecipeCache.Get(itemId);
        if (cached is not null && cached.Count > 0)
            return cached;

        using var scope = serviceProvider.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        var recipes = dbContext.Recipe.Where(r => r.TargetItemId == itemId).ToList();
        return CacheRecipes(recipes);
    }

    public List<RecipePoco> GetRecipesForItemIds(IEnumerable<int> itemIds)
    {
        using var scope = serviceProvider.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        var recipes = dbContext.Recipe.Where(r =>
            itemIds.Any(a => a == r.TargetItemId)).ToList();
        return CacheRecipes(recipes);
    }

    public List<RecipePoco> GetMultiple(IEnumerable<int> itemIds)
    {
        using var scope = serviceProvider.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        var recipes = dbContext.Recipe.Where(r =>
            itemIds.Any(a => a == r.Id)).ToList();
        return CacheRecipes(recipes);
    }

    public List<RecipePoco> GetAll()
    {
        using var scope = serviceProvider.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        var recipes = dbContext.Recipe.ToList();
        return CacheRecipes(recipes);
    }

    public Task FillCache()
    {
        using var scope = serviceProvider.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        var recipes = dbContext.Recipe.ToList();
        _ = CacheRecipes(recipes);
        return Task.CompletedTask;
    }

    private List<RecipePoco> CacheRecipes(List<RecipePoco> recipes)
    {
        foreach (var recipe in recipes)
            recipeCache.Add(recipe.Id, recipe);

        var ids = recipes.Select(r => r.TargetItemId).Distinct().ToList();
        foreach (var itemId in ids)
            itemRecipeCache.Add(itemId, recipes.Where(itemRecipe => itemRecipe.Id == itemId).ToList());

        return recipes;
    }
}