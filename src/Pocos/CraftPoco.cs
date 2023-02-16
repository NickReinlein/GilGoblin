namespace GilGoblin.Pocos;

public class CraftPoco : CraftIngredientPoco
{
    public List<CraftIngredientPoco> Ingredients { get; set; } = new List<CraftIngredientPoco>();

    public CraftPoco(
        IngredientPoco ingredient,
        PricePoco price,
        List<CraftIngredientPoco> ingredients
    ) : base(ingredient, price)
    {
        this.Ingredients = ingredients;
    }
}
