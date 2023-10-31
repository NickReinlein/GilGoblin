using System.Collections.Generic;
using GilGoblin.Database.Pocos;

namespace GilGoblin.Api.Crafting;

public class CraftPoco : CraftIngredientPoco
{
    public List<CraftIngredientPoco> Ingredients { get; set; }

    public CraftPoco(
        IngredientPoco ingredient,
        PricePoco price,
        List<CraftIngredientPoco> ingredients
    ) : base(ingredient, price)
    {
        Ingredients = ingredients;
    }
}