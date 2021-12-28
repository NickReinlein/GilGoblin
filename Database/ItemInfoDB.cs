using GilGoblin.WebAPI;
using Microsoft.EntityFrameworkCore;
using Serilog;
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
                foreach (ItemRecipeHeaderAPI header in web.recipeHeader)
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

        public static ItemInfoDB GetItemInfo(int itemID)
        {
            ItemInfoDB db;
            try
            {
                db = GetItemInfoDB(itemID);
            }
            catch (Exception) { db = null; }
            try
            {
                if (db == null)
                {
                    db = FetchItemInfo(itemID);
                }
                return db;
            }
            catch (Exception ex) {
                Log.Error("Failed to get or fetch item info for item {itemID} with message: {message}", itemID, ex.Message);
            return null;
            }
        }

        public static ItemInfoDB GetItemInfoDB(int itemID)
        {
            if (itemID == 0) { return null; }

            ItemDBContext context = DatabaseAccess.context;
            try
            {
                ItemInfoDB exist = context.itemInfoData
                    .Where(t => (t.itemID == itemID))
                    .Include(t => t.fullRecipes)
                    .FirstOrDefault();
                return exist;
            }
            catch(Exception) { return null; }                        
        }

        public static ItemInfoDB FetchItemInfo(int itemID)
        {
            
            ItemInfoWeb web = ItemInfoWeb.FetchItemInfo(itemID).GetAwaiter().GetResult();
            if (web == null) { return null; }
            ItemInfoDB db = new ItemInfoDB(web);
            return db;
        }
    }

}
