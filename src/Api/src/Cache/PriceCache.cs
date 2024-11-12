using GilGoblin.Database.Pocos;

namespace GilGoblin.Api.Cache;

public interface IPriceCache : IDataCache<(int, int, bool), PricePoco>
{
}

public class PriceCache : DataCache<(int, int, bool), PricePoco>, IPriceCache
{
}