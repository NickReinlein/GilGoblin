using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database;

public class ItemGateway : IItemRepository
{
    private readonly IItemRepository _recipes;

    public ItemGateway(IItemRepository recipes)
    {
        _recipes = recipes;
    }

    public async Task<ItemInfoPoco?> Get(int recipeID) => await _recipes.Get(recipeID);

    public async Task<IEnumerable<ItemInfoPoco?>> GetMultiple(IEnumerable<int> recipeIDs) =>
        await _recipes.GetMultiple(recipeIDs);

    public async Task<IEnumerable<ItemInfoPoco>> GetAll() => await _recipes.GetAll();
}
