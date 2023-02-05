using GilGoblin.Pocos;

namespace GilGoblin.Repository;

public interface IRecipeRepository : IDataRepository<RecipePoco>
{
    Task<IEnumerable<RecipePoco?>> GetRecipesForItem(int id);
}
