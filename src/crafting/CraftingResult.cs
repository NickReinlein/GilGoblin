using GilGoblin.pocos;

namespace GilGoblin.crafting{
    public class CraftingResult : RecipePoco {
        public int RecipeID { get; set ;}
        public int ItemID { get; set; }
        public int Quantity { get; set; } = 1;
        public bool CanHq { get; set; }
        public bool CanQuickSynth { get; set; }
        public List<IngredientPoco> Ingredients { get; set; } = new List<IngredientPoco>();

        public CraftingResult(int recipeID, int itemID, int quantity, bool canHq, bool canQuickSynth, List<IngredientPoco> ingredients)
        {
            RecipeID = recipeID;
            ItemID = itemID;
            Quantity = quantity;
            CanHq = canHq;
            CanQuickSynth = canQuickSynth;
            Ingredients = ingredients;
        }
    }
}