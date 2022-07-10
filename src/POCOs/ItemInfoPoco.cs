using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GilGoblin.POCOs
{
    internal class ItemInfoPoco
    {
        public int itemID { get; set; }
        public int worldID { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int iconID { get; set; }
        public int vendorPrice { get; set; }
        public int stackSize { get; set; }
        public int gatheringId { get; set; }                
        public DateTime lastUpdated { get; set; }
        public ICollection<ItemRecipeHeaderPoco> recipeHeader { get; set; } = new List<ItemRecipeHeaderPoco>();

        [JsonConstructor]
        public ItemInfoPoco(int ID, int IconID, string Name, string Description, int priceMid, int stackSize, int gatheringID, List<ItemRecipeHeaderPoco> Recipes)
        {
            this.itemID = ID;
            this.iconID = IconID;
            this.description = Description;
            this.name = Name;
            this.vendorPrice = priceMid;
            this.stackSize = stackSize;
            this.gatheringId = gatheringID;
            
            if (Recipes != null) 
            { 
                this.recipeHeader = Recipes; 
            }
            else { 
                this.recipeHeader.Clear(); 
            }

        }
        public static async Task<ItemInfoPoco?> FetchItemInfo(int itemId)
        {
            ItemInfoPoco? itemInfo;
            try
            {
                HttpClient client = new HttpClient();
                string url = "https://xivapi.com/Item/" + itemId;
                var content = await client.GetAsync(url);

                return itemInfo = JsonConvert.DeserializeObject<ItemInfoPoco>(content.Content.ReadAsStringAsync().Result);                
            }
            catch (Exception ex)
            {
                Log.Error("Failed to convert item info from JSON:" + ex.Message);
                return null;
            }
        }

    }

}
