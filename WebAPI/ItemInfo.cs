using System.Collections.Generic;

namespace GilGoblin.WebAPI
{
    internal class ItemInfo
    {
        public int item_id { get; set; }
        public string description { get; set; }
        public int icon_id { get; set; }
        public int vendor_price { get; set; }
        public int stack_size { get; set; }

        public int gathering_id { get; set; }

        public List<APIRecipe> recipes { get; set; }

        /// <summary>
        /// Constructor for JSON de-serialization; may add more constructors later
        /// </summary>
        /// <param name="item_id">The item's ID number</param>
        /// <param name="world_name">The world name</param>
        /// <param name="item_name">Optional: Item name</param>
        /// <param name="current_listings">Optional: current listings on the marketboard</param>
        public ItemInfo(int itemID, string name, int priceMid, int stackSize, List<APIRecipe> recipes)
        {
            this.item_id = itemID;
            this.description = name;
            this.vendor_price = priceMid;
            this.stack_size = stackSize;
            //this.gathering_item = 
            this.recipes = recipes;
        }
    }
}
