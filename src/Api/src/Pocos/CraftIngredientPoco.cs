using GilGoblin.Database.Pocos;

namespace GilGoblin.Api.Pocos;

public class CraftIngredientPoco(IngredientPoco ingredient, PricePoco price)
    : IngredientPoco(ingredient.ItemId, ingredient.Quantity, ingredient.RecipeId, ingredient.IsHq)
{
    public PricePoco Price { get; set; } = price;
}
