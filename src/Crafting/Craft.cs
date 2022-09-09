

using GilGoblin.pocos;

namespace GilGoblin.crafting {
    public class Craft : CraftIngredient {
        public List<CraftIngredient> Ingredients { get; set; } = new List<CraftIngredient>();
        public Craft(IngredientPoco ingredient, MarketDataPoco marketData, List<CraftIngredient> ingredients) : base(ingredient, marketData)
        {
            this.Ingredients = ingredients;
        }
    }
}