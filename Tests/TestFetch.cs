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
    public class TestFetch
    {
        /// <summary>
        /// Test the fetching of a full recipe using the recipe's ID
        /// </summary>
        /// <param name="recipeID"></param>        
        /// /// Bomb Frypan
        [Theory, InlineData(230)]
        public void TestFetchRecipe(int recipeID)
        {
            
            RecipeFullWeb thisRecipe 
                = RecipeFullWeb.FetchRecipe(recipeID).GetAwaiter().GetResult();
            Assert.NotNull(thisRecipe);
            Assert.Equal(recipeID, thisRecipe.recipe_id);
            Assert.Equal(2499, thisRecipe.target_item_id);
            Assert.Equal(35706, thisRecipe.icon_id);
            Assert.Equal(1, thisRecipe.result_quantity);
            Assert.True(thisRecipe.CanHq);
            Assert.True(thisRecipe.CanQuickSynth);
        }

        // Iron Ingot        
        [Theory, InlineData(5057)]
        public void TestFetchItemInfo(int itemID)
        {
            ItemInfoWeb web = ItemInfoWeb.FetchItemInfo(itemID).GetAwaiter().GetResult();
            Assert.NotNull(web);
            Assert.Equal(20801, web.iconID);
            Assert.Equal(68, web.vendor_price);
            Assert.Equal(999, web.stack_size);
            Assert.Equal("An ingot of smelted iron.", web.description);
            Assert.Equal("Iron Ingot", web.name);

            //Test gathering ID later?
        }

        // Iron Ingot, Brynhildr
        [Theory, InlineData(5057,34)]
        public void TestFetchMarketData(int itemID, int worldID)
        {
            DateTime testDateTime = new DateTime(2021, 1, 1);
            MarketDataWeb marketDataWeb 
                = MarketDataWeb.FetchMarketData(itemID, worldID).GetAwaiter().GetResult();
            Assert.NotNull(marketDataWeb);            
            // This could be false but there should be a value returned/calculated
            Assert.True(marketDataWeb.averagePrice > 10 && marketDataWeb.averagePrice < 500);
            Assert.Equal(itemID, marketDataWeb.itemID);
            Assert.Equal(worldID, marketDataWeb.worldID);
            Assert.True(marketDataWeb.lastUpdated.CompareTo(DateTime.Now) < 0);
            Assert.True(marketDataWeb.lastUpdated.CompareTo(new DateTime(2020,1,1)) > 0);
            
            // Listings
            Assert.NotEmpty(marketDataWeb.listings);
            MarketListingWeb testListing = marketDataWeb.listings.First();
            Assert.NotNull(testListing);
            Assert.Equal(itemID, testListing.item_id);
            Assert.Equal(worldID, testListing.world_id);
            Assert.True(testListing.qty>0);
            Assert.True(testListing.price>0);
            Assert.True(testListing.timestamp.CompareTo(DateTime.Now.AddDays(1)) < 0);
            Assert.True(testListing.timestamp.CompareTo(testDateTime) > 0);

        }
        

    }
}
