using GilGoblin.Database.Pocos;

namespace GilGoblin.Api.Cache;

public interface IPriceCache : IDataCache<TripleKey, PricePoco>
{
}

public class PriceCache : DataCache<TripleKey, PricePoco>, IPriceCache
{
}