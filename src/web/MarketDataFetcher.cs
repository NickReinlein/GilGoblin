using GilGoblin.Pocos;

namespace GilGoblin.web
{
    public class MarketDataFetcher : IMarketDataWeb
    {
        public MarketDataWebPoco[] FetchMarketDataItems(int worldId, int[] itemIDs)
        {
            List<MarketDataWebPoco> list = new List<MarketDataWebPoco>();
            foreach (var pocoID in itemIDs){
                list.Add(new MarketDataWebPoco(1,pocoID,1,"test","testRealm", 300,200,400, 600,400,800));
            }
            return list.ToArray<MarketDataWebPoco>();
        }
        // WIP 
        //      try
        //     {
        //         List<MarketDataWebPoco> list = new List<MarketDataWebPoco>();
        //         HttpClient client = new HttpClient();
        //         //example: https://universalis.app/api/34/5114%2C5106%2C5057
        //         string url = String.Concat("https://universalis.app/api/", worldId, "/");
        //         foreach (int itemID in itemIDs)
        //         {
        //             url += String.Concat(itemID + "%2C");
        //         }
        //         var content = await client.GetAsync(url);
        //         var json = content.Content.ReadAsStringAsync().Result;

        //         //Deserialize from JSON to the object
        //         MarketDataBulkPoco? bulkData = JsonConvert.DeserializeObject<MarketDataBulkPoco>(json);
        //         if (bulkData != null)
        //         {Properties
        //             list = bulkData.items;
        //         }
        //         return list;
        //     }
        //     catch (Exception ex)
        //     {
        //         Log.Error("Failed to convert market data from JSON:" + ex.Message);
        //         return null;
        //     }
        // }        

    }
}