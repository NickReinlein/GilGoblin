using System.Collections.Generic;
using GilGoblin.Database.Pocos;

namespace GilGoblin.Repository;

public interface IRecipeRepository : IDataRepository<RecipePoco>
{
    IEnumerable<RecipePoco> GetRecipesForItem(int itemId);
}
