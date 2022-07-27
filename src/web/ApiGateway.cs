using GilGoblin.Pocos;

namespace GilGoblin.web
{
    class ApiGateway : IHealthCheck{

        public MarketDataWebPoco[] updateMarketData(int worldID, int[] itemIDs){
            var webResponse = new MarketDataFetcher().FetchMarketDataItems(worldID, itemIDs);
            return webResponse;
        }

        public bool ping(){
            return true;
        }
    }
}