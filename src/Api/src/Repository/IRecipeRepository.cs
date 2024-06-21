using System.Collections.Generic;
using GilGoblin.Database.Pocos;

namespace GilGoblin.Api.Repository;

public interface IRecipeRepository : IDataRepository<RecipePoco>
{
    List<RecipePoco> GetRecipesForItem(int itemId);
    List<RecipePoco> GetRecipesForItemIds(IEnumerable<int> itemIds);
}