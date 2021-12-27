using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GilGoblin.WebAPI
{
    internal class ItemInfoWeb : ItemInfo
    {
        public ICollection<ItemRecipeHeaderAPI> recipeHeader { get; set; } = new List<ItemRecipeHeaderAPI>();

        public ItemInfoWeb() : base() { }

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
        public ItemInfoWeb(int ID, int IconID, string Name, string Description, int priceMid, int stackSize, int gatheringID, List<ItemRecipeHeaderAPI> Recipes)
        {
            this.itemID = ID;
            this.iconID = IconID;
            this.description = Description;
            this.name = Name;
            this.vendor_price = priceMid;
            this.stack_size = stackSize;
            this.gatheringID = gatheringID;
            if (Recipes != null) { this.recipeHeader = Recipes; }
            else { this.recipeHeader.Clear(); }

        }

    }
}
