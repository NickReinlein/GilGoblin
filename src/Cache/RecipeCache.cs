using GilGoblin.Pocos;

namespace GilGoblin.Cache;

public interface IRecipeCache : IDataCache<(int, int), RecipePoco> { }

public class RecipeCache : DataCache<(int, int), RecipePoco>, IRecipeCache { }
