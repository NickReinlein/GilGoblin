using GilGoblin.Pocos;

namespace GilGoblin.Repository;

public interface IPriceRepository
{
    MarketDataPoco Get(int worldID, int id);
    IEnumerable<MarketDataPoco> GetAll(int worldID);
}
