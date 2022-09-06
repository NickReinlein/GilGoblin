using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GilGoblin.Pocos
{
    internal class ItemInfoPoco
    {
        public int ItemID { get; set; }
        public int WorldID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int IconID { get; set; }
        public int VendorPrice { get; set; }
        public int StackSize { get; set; }
        public int GatheringID { get; set; }
        public DateTime LastUpdated { get; set; }
        public ICollection<ItemRecipeHeaderPoco> RecipeHeaders { get; set; } = new List<ItemRecipeHeaderPoco>();

        [JsonConstructor]
        public ItemInfoPoco(int id, int iconID, string name, string description, int priceMid, int stackSize, int gatheringID, List<ItemRecipeHeaderPoco> recipes)
        {
            this.ItemID = id;
            this.IconID = iconID;
            this.Description = description;
            this.Name = name;
            this.VendorPrice = priceMid;
            this.StackSize = stackSize;
            this.GatheringID = gatheringID;

            if (recipes != null)
            {
                this.RecipeHeaders = recipes;
            }
            else
            {
                this.RecipeHeaders.Clear();
            }

        }

        // TODO decouple from Poco
        public async Task<ItemInfoPoco?> FetchItemInfo(int itemId)
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
