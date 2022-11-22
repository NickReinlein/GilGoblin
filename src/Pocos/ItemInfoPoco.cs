using Newtonsoft.Json;
using Serilog;

namespace GilGoblin.Pocos
{
    public class ItemInfoPoco
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int IconID { get; set; }
        public int VendorPrice { get; set; }
        public int StackSize { get; set; }
        public int GatheringID { get; set; }

        [JsonConstructor]
        public ItemInfoPoco(int id, string name, string description, int iconID, int vendorPrice, int stackSize, int gatheringID)
        {
            this.ID = id;
            this.IconID = iconID;
            this.Description = description;
            this.Name = name;
            this.VendorPrice = vendorPrice;
            this.StackSize = stackSize;
            this.GatheringID = gatheringID;
        }

        public ItemInfoPoco()
        {
        }
        public ItemInfoPoco(ItemInfoPoco copyMe)
        {
            this.ID = copyMe.ID;
            this.IconID = copyMe.IconID;
            this.Description = copyMe.Description;
            this.Name = copyMe.Name;
            this.VendorPrice = copyMe.VendorPrice;
            this.StackSize = copyMe.StackSize;
            this.GatheringID = copyMe.GatheringID;
        }

        // TODO decouple from Poco
        public static async Task<ItemInfoPoco?> FetchItemInfo(int itemId)
        {
            ItemInfoPoco? itemInfo;
            try
            {
                var client = new HttpClient();
                var url = "https://xivapi.com/Item/" + itemId;
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
