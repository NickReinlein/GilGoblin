using GilGoblin.Database.Pocos;

namespace GilGoblin.Api.Cache;

public interface IRecipeProfitCache : IDataCache<(int, int), RecipeProfitPoco>
{
}

public class RecipeProfitCache : DataCache<(int, int), RecipeProfitPoco>, IRecipeProfitCache
{
}