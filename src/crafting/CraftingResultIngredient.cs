using GilGoblin.pocos;

namespace GilGoblin.crafting{
    public class CraftingResultIngredient : IngredientPoco {

        public int MarketCost { get; set; }
        public int CraftingCost { get; set; }

        public CraftingResultIngredient(int itemID, int quantity, int recipeID, int marketCost, int craftingCost) : base(itemID, quantity, recipeID)
        {
        }
        public CraftingResultIngredient(CraftingResultIngredient copyMe) : base(copyMe)
        {
            this.MarketCost = copyMe.MarketCost;
            this.CraftingCost = copyMe.CraftingCost;
        }
        
    }
}