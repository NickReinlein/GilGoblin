using System;
using System.Collections.Generic;
using GilGoblin.Database.Pocos;

namespace GilGoblin.Pocos;

public class CraftSummaryPoco : IComparable
{
    public int ItemId { get; set; }
    public int WorldId { get; set; }
    public string Name { get; set; }
    public int? IconId { get; set; }
    public int? ItemLevel { get; set; }
    public int? StackSize { get; set; }
    public int? PriceMid { get; set; }
    public int? PriceLow { get; set; }
    public float AverageListingPrice { get; set; }
    public float AverageSold { get; set; }
    public float CraftingCost { get; set; }
    public RecipePoco Recipe { get; set; }
    public float CraftingProfitVsSold { get; set; }
    public float CraftingProfitVsListings { get; set; }
    public IEnumerable<IngredientPoco> Ingredients { get; set; }

    public CraftSummaryPoco() { }

    public CraftSummaryPoco(
        int worldId,
        int itemId,
        string name,
        int? iconId,
        int? itemLevel,
        int? priceMid,
        int? priceLow,
        int? stackSize,
        float averageListingPrice,
        float averageSold,
        float craftingCost,
        float craftingProfitVsSold,
        float craftingProfitVsListings,
        IEnumerable<IngredientPoco> ingredients
    )
    {
        WorldId = worldId;
        ItemId = itemId;
        Name = name;
        IconId = iconId;
        PriceMid = priceMid;
        PriceLow = priceLow;
        StackSize = stackSize;
        ItemLevel = itemLevel;
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
        WorldId = price.WorldId;
        ItemId = price.ItemId;
        Name = itemInfo.Name;
        IconId = itemInfo.IconId;
        ItemLevel = itemInfo.Level;
        PriceMid = itemInfo.PriceMid;
        PriceLow = itemInfo.PriceLow;
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
            var worldIdComparison = WorldId.CompareTo(otherCraftSummary.WorldId);
            if (worldIdComparison != 0)
                return worldIdComparison;

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

            var priceMidComparison = CraftingProfitVsListings.CompareTo(otherCraftSummary.PriceMid);
            if (priceMidComparison != 0)
                return -1 * priceMidComparison;

            var priceLowComparison = CraftingProfitVsListings.CompareTo(otherCraftSummary.PriceLow);
            if (priceLowComparison != 0)
                return -1 * priceLowComparison;

            return ItemId.CompareTo(otherCraftSummary.ItemId);
        }

        return 0;
    }
}