using GilGoblin.Pocos;

namespace GilGoblin.Cache;

public interface IRecipeCostCache : IDataCache<(int, int), RecipeCostPoco> { }

public class RecipeCostCache : DataCache<(int, int), RecipeCostPoco>, IRecipeCostCache { }
