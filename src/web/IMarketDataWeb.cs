using GilGoblin.Pocos;

namespace GilGoblin.web
{
    public interface IMarketDataWeb
    {
        public MarketDataWebPoco[] FetchMarketDataItems(int worldId, int[] itemIDs);
    }
}