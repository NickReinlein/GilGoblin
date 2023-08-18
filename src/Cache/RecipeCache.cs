using GilGoblin.Pocos;

namespace GilGoblin.Cache;

public interface IRecipeCache : IDataCache<int, RecipePoco> { }

public class RecipeCache : DataCache<int, RecipePoco>, IRecipeCache { }
