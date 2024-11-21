using System;
using GilGoblin.Database.Pocos;

namespace GilGoblin.Api.Pocos;

public class CraftSummaryPoco : IComparable
{
    public int RecipeId { get; set; }
    public int ItemId { get; set; }
    public int WorldId { get; set; }
    public bool IsHq { get; set; }
    public ItemPoco? ItemInfo { get; set; }
    public RecipePoco? Recipe { get; set; }
    public int SalePrice { get; set; }
    public int CraftingCost { get; set; }
    public int Profit { get; set; }
    public DateTimeOffset Updated { get; set; }

    public int CompareTo(object? obj)
    {
        if (obj == null)
            return 1;

        if (obj is not CraftSummaryPoco otherCraftSummary)
            return 0;

        if (WorldId.CompareTo(otherCraftSummary.WorldId) != 0)
            return WorldId.CompareTo(otherCraftSummary.WorldId);

        if (Profit.CompareTo(otherCraftSummary.Profit) != 0)
            return Profit.CompareTo(otherCraftSummary.Profit);

        if (RecipeId.CompareTo(otherCraftSummary.RecipeId) != 0)
            return RecipeId.CompareTo(otherCraftSummary.RecipeId);

        if (ItemId.CompareTo(otherCraftSummary.ItemId) != 0)
            return ItemId.CompareTo(otherCraftSummary.ItemId);

        if (IsHq.CompareTo(otherCraftSummary.IsHq) != 0)
            return IsHq.CompareTo(otherCraftSummary.IsHq);

        if (SalePrice.CompareTo(otherCraftSummary.SalePrice) != 0)
            return SalePrice.CompareTo(otherCraftSummary.SalePrice);

        if (CraftingCost.CompareTo(otherCraftSummary.CraftingCost) != 0)
            return CraftingCost.CompareTo(otherCraftSummary.CraftingCost);

        if (Updated.CompareTo(otherCraftSummary.Updated) != 0)
            return Updated.CompareTo(otherCraftSummary.Updated);

        return 0;
    }
}