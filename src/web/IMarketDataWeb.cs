using GilGoblin.Pocos;

namespace GilGoblin.web
{
    public interface IMarketDataWeb
    {
        public MarketDataWebPoco? FetchMarketData(int worldID, int itemID);
        public MarketDataWebPoco[] FetchMarketDataBulk(int worldId, int[] itemIDs);
    }
}