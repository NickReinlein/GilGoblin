namespace GilGoblin.Pocos;

public class CraftIngredientPoco : IngredientPoco
{
    public PricePoco Price { get; set; }

    public CraftIngredientPoco(IngredientPoco ingredient, PricePoco price)
        : base(ingredient.ItemId, ingredient.Quantity, ingredient.RecipeId)
    {
        this.Price = price;
    }
}
