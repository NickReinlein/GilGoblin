using GilGoblin.pocos;

namespace GilGoblin.crafting {
    public class CraftIngredient : IngredientPoco  {
        public MarketDataPoco MarketData {get; set; }

        public CraftIngredient(IngredientPoco ingredient, MarketDataPoco marketData) : base(ingredient.ItemID,
            ingredient.Quantity, ingredient.RecipeID)
        {
            this.MarketData = marketData;
        }        
    }
}