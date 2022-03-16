using GilGoblin.Finance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GilGoblin.Tests
{
    public class TestGet
    {
        // Iron Ingot, Brynhildr
        [Theory, InlineData(5057, 34)]
        public void TestGetAveragePrice(int itemID, int worldID)
        {
            int avg = Prices.getAveragePrice(itemID, worldID);
            Assert.True(avg > 1);
            Assert.True(avg < 600); //beyond reasonable price
         }

        // Iron Ingot, Brynhildr
        [Theory, InlineData(5057, 34)]
        public void TestCalculateBaseCost(int itemID, int worldID)
        {
            int baseCost = Cost.GetBaseCost(itemID, worldID);
            Assert.True(baseCost > 1);
            Assert.True(baseCost < 600);
        }

        // Iron Ingot, Brynhildr
        [Theory, InlineData(5057, 34)]
        public void TestGetMinCost(int itemID, int worldID)
        {
            int minCost = Cost.GetMinCost(itemID, worldID);
            Assert.True(minCost > 1);
            Assert.True(minCost < 600);
        }

        // Iron Ingot, Brynhildr
        [Theory, InlineData(5057, 34)]
        public void TestCalculateCraftingCost(int itemID, int worldID)
        {
            int craftingCost = Cost.GetCraftingCost(itemID, worldID);
            Assert.True(craftingCost > 1);
            Assert.True(craftingCost < 999999);
        }
    }
}
