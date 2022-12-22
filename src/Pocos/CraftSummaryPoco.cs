namespace GilGoblin.Pocos;

public class CraftSummaryPoco
{
    public int WorldID { get; set; }
    public int ItemID { get; set; }
    public string Name { get; set; } = "";
    public int VendorPrice { get; set; }
    public float AverageListingPrice { get; set; }
    public float AverageSold { get; set; }
    public float CraftingCost { get; set; }
    public float CraftingProfitVsSold { get; set; }
    public float CraftingProfitVsListings { get; set; }
    public List<IngredientPoco> Ingredients { get; set; } = new List<IngredientPoco>();

    public CraftSummaryPoco() { }

    public CraftSummaryPoco(
        int worldID,
        int itemID,
        string name,
        int vendorPrice,
        float averageListingPrice,
        float averageSold,
        float craftingCost,
        float craftingProfitVsSold,
        float craftingProfitVsListings,
        List<IngredientPoco> ingredients
    )
    {
        this.WorldID = worldID;
        this.ItemID = itemID;
        this.Name = name;
        this.VendorPrice = vendorPrice;
        this.AverageListingPrice = averageListingPrice;
        this.AverageSold = averageSold;
        this.CraftingCost = craftingCost;
        this.CraftingProfitVsSold = craftingProfitVsSold;
        this.CraftingProfitVsListings = craftingProfitVsListings;
        this.Ingredients = ingredients;
    }
}
