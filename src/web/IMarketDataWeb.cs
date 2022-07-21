using GilGoblin.Pocos;

namespace GilGoblin.web
{
    public interface IMarketDataWeb
    {
        public MarketDataWebPoco FetchMarketData(int itemID, int worldID);
        public MarketDataWebPoco[] FetchMarketDataBulk(int[] itemIDs, int worldId);
    }
}