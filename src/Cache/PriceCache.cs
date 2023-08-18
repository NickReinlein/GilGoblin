using GilGoblin.Pocos;

namespace GilGoblin.Cache;

public interface IPriceCache : IDataCache<(int, int), PricePoco> { }

public class PriceCache : DataCache<(int, int), PricePoco>, IPriceCache { }
