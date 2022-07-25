using GilGoblin.Pocos;

namespace GilGoblin.web
{
    public class MarketDataFetcher : IMarketDataWeb
    {
        public MarketDataWebPoco? FetchMarketData(int worldID, int itemID)
        {
            int[] itemAsArray = { itemID };
            var response = FetchMarketDataBulk(worldID, itemAsArray);
            return response[0];
        }

        public MarketDataWebPoco[] FetchMarketDataBulk(int worldId,int[] itemIDs)
        {
            return Array.Empty<MarketDataWebPoco>();
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




// //interface CurrentlyShownView {