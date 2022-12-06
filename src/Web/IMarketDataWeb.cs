using GilGoblin.Pocos;

namespace GilGoblin.Web;

public interface IMarketDataWeb
{
    public IEnumerable<MarketDataPoco> FetchMarketData(int worldId, IEnumerable<int> itemIDs);
}