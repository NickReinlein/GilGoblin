using GilGoblin.Pocos;

namespace GilGoblin.Database
{
    public interface IMarketDataDB
    {
        public IEnumerable<MarketDataPoco> GetMarketData(int worldId, IEnumerable<int> itemIDs);
    }
}