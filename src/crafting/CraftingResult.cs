using GilGoblin.pocos;

namespace GilGoblin.crafting
{
    public class CraftingResult
    {
        public pocos.IngredientPoco Ingredient { get; set; }
        public int Cost { get; set; }
        public CraftingResult(pocos.IngredientPoco ingredient, int cost)
        {
            Ingredient = ingredient;
            Cost = cost;
        }
    }
}
