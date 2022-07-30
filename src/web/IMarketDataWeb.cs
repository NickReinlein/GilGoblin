using GilGoblin.Pocos;

namespace GilGoblin.web
{
    public interface IMarketDataWeb
    {
        public IEnumerable<MarketDataWebPoco> FetchMarketDataItems(int worldId, IEnumerable<int> itemIDs);
        //TODO other POCOS should have methods here to aggregate market data
    }
}