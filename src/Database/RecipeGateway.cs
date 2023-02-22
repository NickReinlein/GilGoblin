using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database;

public class RecipeGateway : IRecipeRepository
{
    private readonly GoblinDatabase _database;
    private readonly ILogger<RecipeGateway> _logger;

    public RecipeGateway(GoblinDatabase database, ILogger<RecipeGateway> logger)
    {
        _database = database;
        _logger = logger;
    }

    public async Task<RecipePoco?> Get(int recipeID)
    {
        _logger.LogInformation("Getting recipe {ID}", recipeID);
        using var context = await GetContext();
        var recipePoco = context?.Recipe?.FirstOrDefault(x => x.ID == recipeID);
        return recipePoco;
    }

    public async Task<IEnumerable<RecipePoco?>> GetRecipesForItem(int itemID)
    {
        _logger.LogInformation("Getting recipes for item {ID}", itemID);
        using var context = await GetContext();
        return context?.Recipe?.Where(i => i.TargetItemID == itemID).ToList()
            ?? new List<RecipePoco>();
    }

    public async Task<IEnumerable<RecipePoco?>> GetMultiple(IEnumerable<int> recipeIDs)
    {
        _logger.LogInformation("Getting {Number} recipes", recipeIDs.Count());
        using var context = await GetContext();
        return context?.Recipe?.Where(i => recipeIDs.Contains(i.ID)).ToList()
            ?? new List<RecipePoco>();
    }

    public async Task<IEnumerable<RecipePoco>> GetAll()
    {
        _logger.LogInformation("Getting all recipes");
        using var context = await GetContext();
        return context?.Recipe?.ToList() ?? new List<RecipePoco>();
    }

    private async Task<GilGoblinDbContext?> GetContext() => await _database.GetContextAsync();
}
