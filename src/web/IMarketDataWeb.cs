using GilGoblin.pocos;

namespace GilGoblin.web
{
    public interface IMarketDataWeb
    {
        public IEnumerable<MarketDataPoco> FetchMarketDataItems(int worldId, IEnumerable<int> itemIDs);
        //TODO other POCOS should have methods here to aggregate market data
    }
}