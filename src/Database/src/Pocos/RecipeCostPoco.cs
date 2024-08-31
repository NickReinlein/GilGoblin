namespace GilGoblin.Database.Pocos;

public class RecipeCostPoco : BaseRecipeValue
{
    public int? AverageSalePriceCost { get; set; }
    public int? MinListingPriceCost { get; set; }
    public int? RecentPurchaseCost { get; set; }
}