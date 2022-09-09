using GilGoblin.Pocos;

namespace GilGoblin.Web
{
    public interface IMarketDataGateway
    {
        public IEnumerable<MarketDataPoco> FetchMarketDataItems(int worldId, IEnumerable<int> itemIDs);
        public IEnumerable<MarketDataPoco> GetMarketDataItems(int worldId, IEnumerable<int> itemIDs);
    }
}