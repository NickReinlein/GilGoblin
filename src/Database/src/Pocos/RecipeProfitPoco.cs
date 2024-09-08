namespace GilGoblin.Database.Pocos;

using System;

public record RecipeProfitPoco
{
    public int RecipeId { get; set; }
    public int WorldId { get; set; }
    public bool IsHq { get; set; }
    public decimal? AverageSaleProfit { get; set; }
    public decimal? MinListingProfit { get; set; }
    public decimal? RecentPurchaseProfit { get; set; }
    public DateTime Updated { get; set; }
}