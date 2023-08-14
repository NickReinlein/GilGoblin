using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GilGoblin.Pocos;

public class CraftSummaryPoco
{
    public int WorldID { get; set; }
    public int ItemID { get; set; }
    public string Name { get; set; } = "";
    public int VendorPrice { get; set; }
    public int IconID { get; set; }
    public int StackSize { get; set; }
    public float AverageListingPrice { get; set; }
    public float AverageSold { get; set; }
    public float CraftingCost { get; set; }
    public RecipePoco Recipe { get; }
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

    [JsonConstructor]
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
}
