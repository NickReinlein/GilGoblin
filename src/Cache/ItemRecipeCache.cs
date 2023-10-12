using System.Collections.Generic;
using GilGoblin.Database.Pocos;

namespace GilGoblin.Cache;

public interface IItemRecipeCache : IDataCache<int, List<RecipePoco>> { }

public class ItemRecipeCache : DataCache<int, List<RecipePoco>>, IItemRecipeCache { }
