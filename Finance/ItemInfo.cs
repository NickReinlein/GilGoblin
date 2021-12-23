using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GilGoblin.WebAPI
{
    internal class ItemInfo
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [ForeignKey("item_id")]
        [InverseProperty("MarketDataDB")]                
        public int item_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int icon_id { get; set; }
        public int vendor_price { get; set; }
        public int stack_size { get; set; }
        public int gathering_id { get; set; }

        public ICollection<ItemRecipeAPI> recipes { get; set; } = new List<ItemRecipeAPI>();

        public ItemInfo() { }

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
        [JsonConstructor]
        public ItemInfo(int ID, int IconID, string Name, string Description, int priceMid, int stackSize, int gatheringID, List<ItemRecipeAPI> Recipes)
        {
            this.item_id = ID;
            this.icon_id = IconID;
            this.description = Description;
            this.name = Name;
            this.vendor_price = priceMid;
            this.stack_size = stackSize;
            this.gathering_id = gatheringID;
            if (Recipes != null){ this.recipes = Recipes; }
            else { this.recipes.Clear(); }
        }

    }
}
