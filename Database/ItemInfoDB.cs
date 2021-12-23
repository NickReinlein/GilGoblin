using GilGoblin.WebAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GilGoblin.Database
{
    internal class ItemInfoDB : ItemInfo
    {
        public ICollection<RecipeDB> fullRecipes { get; set; }
            = new List<RecipeDB>();

        public ItemInfoDB() : base() { }

        public ItemInfoDB(ItemInfoWeb web) : base() 
        { 
            this.itemID = web.itemID;
            this.name = web.name;
            this.description = web.description;
            this.stack_size = web.stack_size;
            this.iconID = web.iconID;
            this.gatheringID = web.gatheringID;
            this.vendor_price = web.vendor_price;

            if (web.recipeHeader != null)
            {
                foreach(ItemRecipeHeaderAPI header in web.recipeHeader)
                {
                    RecipeFullWeb thisRecipe 
                        = RecipeFullWeb.FetchRecipe(header.recipe_id).GetAwaiter().GetResult();
                    if (thisRecipe != null)
                    {
                        this.fullRecipes.Add(thisRecipe.convertToDB());
                    }
                }
            }
        }
    }

}
