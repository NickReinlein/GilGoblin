using GilGoblin.Database.Pocos;

namespace GilGoblin.Api.Cache;

public interface IRecipeCache : IDataCache<int, RecipePoco> { }

public class RecipeCache : DataCache<int, RecipePoco>, IRecipeCache { }
