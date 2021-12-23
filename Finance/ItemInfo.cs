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

        public ItemInfo() { }
    }

    internal class ItemInfoWeb : ItemInfo
    { 
        public ICollection<ItemRecipeShortAPI> recipes { get; set; } = new List<ItemRecipeShortAPI>();

        public ItemInfoWeb() : base () { }

        /// <summary>
        /// Constructor for JSON de-serialization of item info (FFXIVAPI)
        /// </summary>
        /// <param name="ID">The item's ID number</param>
        /// <param name="Name">Optional: Item name</param>
        /// <param name="Description">Optional: Item description</param>
        /// <param name="stackSize">Stack size of the sale</param>
        /// <param name="gatheringID">ID of the gathering class</param>       
        /// <param name="priceMid">Price for selling to vendor</param>
        /// <param name="Recipes">Crafting recipes to make the item (represented by a Tree data structure)</param>
        /// 
        [JsonConstructor]
        public ItemInfoWeb(int ID, int IconID, string Name, string Description, int priceMid, int stackSize, int gatheringID, List<ItemRecipeShortAPI> Recipes)
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
