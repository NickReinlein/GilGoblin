using GilGoblin.pocos;

namespace GilGoblin.web
{
    public class MarketDataGateway : IMarketDataGateway
    {
        public IEnumerable<MarketDataPoco> FetchMarketDataItems(int worldId, IEnumerable<int> itemIDs)
        {
            var uniqueItemIDs = itemIDs.Distinct().ToArray();
            //logger.LogWarning("The person {PersonId} could not be found.", personId);

            var returnUniqueItems = uniqueItemIDs.Select(x => new MarketDataPoco(x,1,1,"fake","fakeRealm", 3,2,4,6,4,8));

            return returnUniqueItems;
        }

        public IEnumerable<MarketDataPoco> GetMarketDataItems(int worldId, IEnumerable<int> itemIDs)
        {
            return FetchMarketDataItems(worldId,itemIDs);
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