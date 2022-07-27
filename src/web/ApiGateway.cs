using GilGoblin.Pocos;

namespace GilGoblin.web
{
    class ApiGateway : IHealthCheck{

        public MarketDataWebPoco[] updateMarketData(int worldID, int[] itemIDs){
            return new MarketDataFetcher().FetchMarketDataItems(worldID, itemIDs);
        }

        public bool ping(){
            return true;
        }
    }
}