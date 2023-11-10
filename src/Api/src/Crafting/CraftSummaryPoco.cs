using System;
using System.Collections.Generic;
using GilGoblin.Database.Pocos;

namespace GilGoblin.Api.Crafting;

public class CraftSummaryPoco : IComparable
{
    public int ItemId { get; set; }
    public int WorldId { get; set; }
    public ItemPoco ItemInfo { get; set; }
    public RecipePoco Recipe { get; set; }
    public int AverageListingPrice { get; set; }
    public int AverageSold { get; set; }
    public int RecipeCost { get; set; }
    public int RecipeProfitVsSold { get; set; }
    public int RecipeProfitVsListings { get; set; }
    public IEnumerable<IngredientPoco> Ingredients { get; set; }
    public DateTimeOffset Updated { get; set; }

    public CraftSummaryPoco() { }

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
        ItemInfo = item;
        Recipe = recipe;
        RecipeCost = recipeCost;
        AverageListingPrice = (int)price.AverageListingPrice;
        AverageSold = (int)price.AverageSold;
        RecipeProfitVsListings = (int)price.AverageListingPrice - recipeCost;
        RecipeProfitVsSold = (int)price.AverageSold - recipeCost;
        Ingredients = ingredients ?? new List<IngredientPoco>();
        Updated = new DateTime(price.LastUploadTime).ToUniversalTime();
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

        var profitVsSoldComparison = RecipeProfitVsSold.CompareTo(otherCraftSummary.RecipeProfitVsSold);
        if (profitVsSoldComparison != 0)
            return -1 * profitVsSoldComparison;

        var profitVsListingsComparison = RecipeProfitVsListings.CompareTo(otherCraftSummary.RecipeProfitVsListings);
        if (profitVsListingsComparison != 0)
            return -1 * profitVsListingsComparison;

        var priceMidComparison = ItemInfo.PriceMid.CompareTo(otherCraftSummary.ItemInfo.PriceMid);
        if (priceMidComparison != 0)
            return -1 * priceMidComparison;

        var priceLowComparison = ItemInfo.PriceLow.CompareTo(otherCraftSummary.ItemInfo.PriceLow);
        if (priceLowComparison != 0)
            return -1 * priceLowComparison;

        return ItemId.CompareTo(otherCraftSummary.ItemId);
    }
}