using System.Collections.Generic;
using System.Threading.Tasks;
using GilGoblin.Pocos;

namespace GilGoblin.Repository;

public interface IRecipeRepository : IDataRepository<RecipePoco>
{
    Task<IEnumerable<RecipePoco?>> GetRecipesForItem(int id);
}
