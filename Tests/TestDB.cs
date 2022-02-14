using GilGoblin.Database;
using GilGoblin.WebAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GilGoblin.Tests
{
    public class TestDB
    {
        /// <summary>
        /// Test to fetch and then convert from web format to database format
        /// </summary>
        /// <param name="recipeID"></param>
        [Theory, InlineData(230)]
        public void TestFetchRecipeAndConvertToDB(int recipeID)
        {
            RecipeFullWeb thisRecipe
                = RecipeFullWeb.FetchRecipe(recipeID).GetAwaiter().GetResult();
            Assert.NotNull(thisRecipe);
            Assert.Equal(recipeID, thisRecipe.recipe_id);

            RecipeDB converted = thisRecipe.convertToDB();
            Assert.Equal(recipeID, converted.recipe_id);
            Assert.Equal(thisRecipe.ingredients.Count, converted.ingredients.Count);
            Assert.Equal(thisRecipe.target_item_id, converted.target_item_id);
            Assert.Equal(thisRecipe.recipe_id, converted.recipe_id);
            Assert.Equal(thisRecipe.result_quantity, converted.result_quantity);
            Assert.Equal(thisRecipe.CanHq, converted.CanHq);
            Assert.Equal(thisRecipe.CanQuickSynth, converted.CanQuickSynth);
        }


        // Iron Ingot, Brynhildr
        [Theory, InlineData(5057, 34)]
        public void TestFetchMarketDataAndConvertToDB(int itemID, int worldID)
        {
            DateTime testDateTime = new DateTime(2021, 1, 1);
            MarketDataDB db = new MarketDataDB(itemID, worldID);                            
            Assert.NotNull(db);
            Assert.True(db.averagePrice > 10 && db.averagePrice < 500);
            Assert.Equal(itemID, db.itemID);
            Assert.Equal(worldID, db.worldID);
            Assert.True(db.lastUpdated.CompareTo(DateTime.Now) < 0);
            Assert.True(db.lastUpdated.CompareTo(new DateTime(2020, 1, 1)) > 0);

            // Listings
            Assert.NotEmpty(db.listings);
            MarketListingDB listing = db.listings.First();
            Assert.NotNull(listing);
            Assert.Equal(itemID, listing.item_id);
            Assert.Equal(worldID, listing.world_id);
            Assert.True(listing.qty > 0);
            Assert.True(listing.price > 0);
            Assert.True(listing.timestamp.CompareTo(DateTime.Now.AddDays(1)) < 0);
            Assert.True(listing.timestamp.CompareTo(testDateTime) > 0);
        }

        [Fact]
        public void TestGetAllCraftingItemIDsViaRecipeLookup()
        {
            var craftingList = CraftingList.getListOfAllCraftableItemIDs();
            Assert.NotNull(craftingList);
            Assert.True(craftingList.Count > 7000);

            var craftingListArmorer = CraftingList.getListOfCraftableItemIDsByClass(CraftingClass.armorer);
            Assert.NotNull(craftingListArmorer);
            Assert.True(craftingListArmorer.Count > 300);

        }

    }
}
