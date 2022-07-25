using GilGoblin.Pocos;

namespace GilGoblin.web
{
    class ApiGateway{
        public MarketDataWebPoco? getMarketDataSingle(int worldID, int itemID){
            return new MarketDataFetcher().FetchMarketData(worldID, itemID);
        }

        public MarketDataWebPoco[] getMarketDataBulk(int worldID, int[] itemIDs){
            return new MarketDataFetcher().FetchMarketDataBulk(worldID, itemIDs);
        }
    }
}