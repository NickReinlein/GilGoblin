namespace GilGoblin.Pocos;

public class CraftIngredientPoco : IngredientPoco
{
    public PricePoco Price { get; set; }

    public CraftIngredientPoco(IngredientPoco ingredient, PricePoco price)
        : base(ingredient.ItemID, ingredient.Quantity, ingredient.RecipeID)
    {
        this.Price = price;
    }
}
