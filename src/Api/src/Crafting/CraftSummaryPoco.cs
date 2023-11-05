using System;
using System.Collections.Generic;
using GilGoblin.Database.Pocos;

namespace GilGoblin.Api.Crafting;

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
    public int AverageListingPrice { get; set; }
    public int AverageSold { get; set; }
    public int RecipeCost { get; set; }
    public RecipePoco Recipe { get; set; }
    public int CraftingProfitVsSold { get; set; }
    public int CraftingProfitVsListings { get; set; }
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
        int averageListingPrice,
        int averageSold,
        int recipeCost,
        int craftingProfitVsSold,
        int craftingProfitVsListings,
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
        RecipeCost = recipeCost;
        CraftingProfitVsSold = craftingProfitVsSold;
        CraftingProfitVsListings = craftingProfitVsListings;
        Ingredients = ingredients ?? new List<IngredientPoco>();
    }

    public CraftSummaryPoco(
        PricePoco price,
        ItemPoco item,
        int recipeCost,
        RecipePoco recipe,
        IEnumerable<IngredientPoco> ingredients
    )
    {
        WorldId = price.WorldId;
        ItemId = price.ItemId;
        Name = item.Name;
        IconId = item.IconId;
        ItemLevel = item.Level;
        PriceMid = item.PriceMid;
        PriceLow = item.PriceLow;
        StackSize = item.StackSize;
        AverageListingPrice = (int)price.AverageListingPrice;
        AverageSold = (int)price.AverageSold;
        RecipeCost = recipeCost;
        Recipe = recipe;
        CraftingProfitVsListings = (int)price.AverageListingPrice - recipeCost;
        CraftingProfitVsSold = (int)price.AverageSold - recipeCost;
        Ingredients = ingredients ?? new List<IngredientPoco>();
    }

    public int CompareTo(object obj)
    {
        if (obj == null)
            return 1;

        if (obj is not CraftSummaryPoco otherCraftSummary)
            return 0;

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
}