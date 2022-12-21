namespace GilGoblin.Pocos;

public class CraftIngredientPoco : IngredientPoco
{
    public MarketDataPoco MarketData { get; set; }

    public CraftIngredientPoco(IngredientPoco ingredient, MarketDataPoco marketData)
        : base(ingredient.ItemID, ingredient.Quantity, ingredient.RecipeID)
    {
        this.MarketData = marketData;
    }
}
