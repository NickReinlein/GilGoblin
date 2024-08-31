namespace GilGoblin.Database.Pocos;

public class RecipeProfitPoco : BaseRecipeValue
{
    public int? AverageSalePriceProfit { get; set; }
    public int? MinListingPriceProfit { get; set; }
    public int? RecentPurchasePrice { get; set; }
}