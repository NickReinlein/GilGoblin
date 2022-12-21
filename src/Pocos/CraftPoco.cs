namespace GilGoblin.Pocos;

public class CraftPoco : CraftIngredientPoco
{
    public List<CraftIngredientPoco> Ingredients { get; set; } = new List<CraftIngredientPoco>();

    public CraftPoco(
        IngredientPoco ingredient,
        MarketDataPoco marketData,
        List<CraftIngredientPoco> ingredients
    ) : base(ingredient, marketData)
    {
        this.Ingredients = ingredients;
    }
}
