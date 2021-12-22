using System.Collections.Generic;

namespace GilGoblin.WebAPI
{
    internal class ItemInfo
    {
        public int item_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int icon_id { get; set; }
        public int vendor_price { get; set; }
        public int stack_size { get; set; }
        public int gathering_id { get; set; }

        public List<ItemRecipeAPI> recipes { get; set; }

        /// <summary>
        /// Constructor for JSON de-serialization of item info (FFXIVAPI)
        /// </summary>
        /// <param name="item_id">The item's ID number</param>
        /// <param name="Name">Optional: Item name/description</param>
        /// <param name="stack_size">Stack size of the sale</param>
        /// <param name="gathering_id">ID of the gathering class</param>       
        /// <param name="vendor_price">Price for selling to vendor</param>
        /// <param name="recipes">Crafting recipes to make the item (represented by a Tree data structure)</param>
        /// 
        public ItemInfo(int ID, int IconID, string Name, string Description, int priceMid, int stackSize, int gatheringID, List<ItemRecipeAPI> recipes)
        {
            this.item_id = ID;
            this.icon_id = IconID;
            this.description = Description;
            this.name = Name;
            this.vendor_price = priceMid;
            this.stack_size = stackSize;
            this.gathering_id = gatheringID;
            this.recipes = recipes;
        }
    }
}
