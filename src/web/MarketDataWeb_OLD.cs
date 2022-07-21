// using Newtonsoft.Json;
// using Serilog;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Net.Http;
// using System.Threading.Tasks;

// namespace GilGoblin.Pocos
// {
//     internal class MarketDataWebPoco_OLD
//     {
//         public int itemID { get; set; }
//         public int worldID { get; set; }
//         public float averagePrice { get; set; }
//         public DateTime lastUpdated { get; set; }
//         public ICollection<MarketListingPoco> listings { get; set; } = new List<MarketListingPoco>();
//         public static int listingsToRead = 20;

//         [JsonConstructor]
//         public MarketDataWebPoco(int itemID, int worldID, long lastUploadTime,
//                                ICollection<MarketListingPoco> listings) : base()
//         {
//             if (itemID == 0 || worldID == 0 || lastUploadTime == 0)
//             {
//                 const string MessageTemplate 
//                     = "Incorrect/missing parameters/arguments coming from the web response. ItemID:{itemID}, worldID:{worldID},lastUpLoadTime:{last}";
//                 Log.Error(MessageTemplate, itemID, this.worldID, lastUploadTime);
//             }
            
//             this.itemID = itemID;
//             this.worldID = worldID;
//             this.lastUpdated = GeneralFunctions.ConvertLongUnixMillisecondsToDateTime(lastUploadTime);

//             this.listings = listings.OrderByDescending(i => i.timestamp).Take(listingsToRead).ToList();

//             foreach (MarketListingPoco listing in this.listings)
//             {
//                 listing.itemID = itemID;
//                 listing.worldID = worldID;
//             }

//             this.averagePrice = (int)CalculateAveragePriceFromListings();

//         }

//         public static async Task<MarketDataWebPoco?> FetchMarketData(int itemID, int worldID)
//         {
//             try
//             {
//                 HttpClient client = new HttpClient();
//                 string url = "https://universalis.app/api/" + worldID + "/" + itemID;
//                 var content = await client.GetAsync(url);

//                 //Deserialize from JSON to the object
//                 MarketDataWebPoco? marketData = JsonConvert.DeserializeObject<MarketDataWebPoco>(content.Content.ReadAsStringAsync().Result);
//                 return marketData;
//             }
//             catch (Exception ex)
//             {
//                 Log.Error("Failed to convert market data from JSON:" + ex.Message);
//                 return null;
//             }
//         }

//         public static async Task<List<MarketDataWebPoco>?> FetchMarketDataBulk(List<int> itemIDs, int world_id)
//         {
//             try
//             {
//                 List<MarketDataWebPoco> list = new List<MarketDataWebPoco>();
//                 HttpClient client = new HttpClient();
//                 //example: https://universalis.app/api/34/5114%2C5106%2C5057
//                 string url = String.Concat("https://universalis.app/api/", world_id, "/");
//                 foreach (int itemID in itemIDs)
//                 {
//                     url += String.Concat(itemID + "%2C");
//                 }
//                 var content = await client.GetAsync(url);
//                 var json = content.Content.ReadAsStringAsync().Result;

//                 //Deserialize from JSON to the object
//                 MarketDataBulkPoco? bulkData = JsonConvert.DeserializeObject<MarketDataBulkPoco>(json);
//                 if (bulkData != null)
//                 {Properties
//                     list = bulkData.items;
//                 }
//                 return list;
//             }
//             catch (Exception ex)
//             {
//                 Log.Error("Failed to convert market data from JSON:" + ex.Message);
//                 return null;
//             }
//         }

//         private int CalculateAveragePriceFromListings()
//         {
//             if (listings.Count == 0) { return 0; }
//             int average_Price = 0;
//             uint total_Qty = 0;
//             ulong total_Gil = 0;
//             try
//             {
//                 foreach (MarketListingPoco listing in listings)
//                 {
//                     if ((listing.quantity > 1 && listing.price > 200000) ||
//                         (listing.quantity == 1 && listing.price > 400000))
//                     {
//                         continue; //Exclude crazy prices
//                     }

//                     total_Qty += ((uint)listing.quantity);
//                     total_Gil += ((ulong)(listing.price * listing.quantity));
//                 }
//                 if (total_Qty > 0)
//                 {
//                     float average_Price_f = total_Gil / total_Qty;
//                     average_Price = (int)Math.Round(average_Price_f);
//                 }
//             }
//             catch (Exception ex)
//             {
//                 Log.Error("Error calculating average price for item {itemID} world {worldID}. Message: {message}", this.itemID, this.worldID, ex.Message);
//                 average_Price = 0;
//             }
//             return average_Price;
//         }            
//     }

//     internal class MarketDataBulkPoco
//     {
//         public List<int> itemIDs { get; set; } = new List<int>();
//         public List<MarketDataWebPoco> items { get; set; } = new List<MarketDataWebPoco>();

//         [JsonConstructor]
//         public MarketDataBulkPoco(ICollection<int> itemIDs, 
//                                  ICollection<MarketDataWebPoco> items): base()
//         {
//             if (itemIDs != null) { this.itemIDs = new List<int>(itemIDs); }
//             if (items != null) { this.items = new List<MarketDataWebPoco>(items); }
//         }
//     }

// }