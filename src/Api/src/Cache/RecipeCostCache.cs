using GilGoblin.Database.Pocos;

namespace GilGoblin.Api.Cache;

public interface IRecipeCostCache : IDataCache<(int, int), RecipeCostPoco>
{
}

public class RecipeCostCache : DataCache<(int, int), RecipeCostPoco>, IRecipeCostCache
{
}