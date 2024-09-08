using System;

namespace GilGoblin.Database.Pocos;

public record RecipeCostPoco
{
    public int RecipeId { get; set; }
    public int WorldId { get; set; }
    public bool IsHq { get; set; }
    public decimal? AverageSaleCost { get; set; }
    public decimal? MinListingCost { get; set; }
    public decimal? RecentPurchaseCost { get; set; }
    public DateTime Updated { get; set; }
}