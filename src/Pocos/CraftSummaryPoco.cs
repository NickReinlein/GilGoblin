using System;
using System.Collections.Generic;

namespace GilGoblin.Pocos;

public class CraftSummaryPoco : IComparable
{
    public int WorldID { get; set; }
    public int ItemID { get; set; }
    public string Name { get; set; }
    public int VendorPrice { get; set; }
    public int IconID { get; set; }
    public int StackSize { get; set; }
    public float AverageListingPrice { get; set; }
    public float AverageSold { get; set; }
    public float CraftingCost { get; set; }
    public RecipePoco Recipe { get; set; }
    public float CraftingProfitVsSold { get; set; }
    public float CraftingProfitVsListings { get; set; }
    public IEnumerable<IngredientPoco> Ingredients { get; set; }

    public CraftSummaryPoco() { }

    public CraftSummaryPoco(
        int worldID,
        int itemID,
        string name,
        int vendorPrice,
        int iconID,
        int stackSize,
        float averageListingPrice,
        float averageSold,
        float craftingCost,
        float craftingProfitVsSold,
        float craftingProfitVsListings,
        IEnumerable<IngredientPoco> ingredients
    )
    {
        WorldID = worldID;
        ItemID = itemID;
        Name = name;
        VendorPrice = vendorPrice;
        IconID = iconID;
        StackSize = stackSize;
        AverageListingPrice = averageListingPrice;
        AverageSold = averageSold;
        CraftingCost = craftingCost;
        CraftingProfitVsSold = craftingProfitVsSold;
        CraftingProfitVsListings = craftingProfitVsListings;
        Ingredients = ingredients ?? new List<IngredientPoco>();
    }

    public CraftSummaryPoco(
        PricePoco price,
        ItemInfoPoco itemInfo,
        float craftingCost,
        RecipePoco recipe,
        IEnumerable<IngredientPoco> ingredients
    )
    {
        WorldID = price.WorldID;
        ItemID = price.ItemID;
        Name = itemInfo.Name;
        VendorPrice = itemInfo.VendorPrice;
        IconID = itemInfo.IconID;
        StackSize = itemInfo.StackSize;
        AverageListingPrice = price.AverageListingPrice;
        AverageSold = price.AverageSold;
        CraftingCost = craftingCost;
        Recipe = recipe;
        CraftingProfitVsListings = price.AverageListingPrice - craftingCost;
        CraftingProfitVsSold = price.AverageSold - craftingCost;
        Ingredients = ingredients ?? new List<IngredientPoco>();
    }

    public int CompareTo(object obj)
    {
        if (obj == null)
            return 1;

        if (obj is CraftSummaryPoco otherCraftSummary)
        {
            var worldIDComparison = WorldID.CompareTo(otherCraftSummary.WorldID);
            if (worldIDComparison != 0)
                return worldIDComparison;

            var profitVsSoldComparison = CraftingProfitVsSold.CompareTo(
                otherCraftSummary.CraftingProfitVsSold
            );
            if (profitVsSoldComparison != 0)
                return -1 * profitVsSoldComparison;

            var profitVsListingsComparison = CraftingProfitVsListings.CompareTo(
                otherCraftSummary.CraftingProfitVsListings
            );
            if (profitVsListingsComparison != 0)
                return -1 * profitVsListingsComparison;

            var vendorPriceComparison = VendorPrice.CompareTo(otherCraftSummary.VendorPrice);
            if (vendorPriceComparison != 0)
                return -1 * vendorPriceComparison;

            return ItemID.CompareTo(otherCraftSummary.ItemID);
        }

        return 0;
    }
}
