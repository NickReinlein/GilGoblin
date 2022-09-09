using GilGoblin.Pocos;

namespace GilGoblin.Crafting
{
    public class CraftIngredient : IngredientPoco
    {
        public MarketDataPoco MarketData { get; set; }

        public CraftIngredient(IngredientPoco ingredient, MarketDataPoco marketData) : base(ingredient.ItemID,
            ingredient.Quantity, ingredient.RecipeID)
        {
            this.MarketData = marketData;
        }
    }
}