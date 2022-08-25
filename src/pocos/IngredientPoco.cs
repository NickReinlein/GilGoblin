using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GilGoblin.pocos
{
    public class IngredientPoco
    {
        // Primary key would now be recipeID & ItemID
        [Key]
        public int RecipeID { get; set; }
        [Key]
        public int ItemID { get; set; }
        public int Quantity { get; set; }

        public IngredientPoco(int itemID, int quantity, int recipeID)
        {
            this.ItemID = itemID;
            this.Quantity = quantity;
            this.RecipeID = recipeID;
        }

        public IngredientPoco(IngredientPoco copyMe)
        {
            this.RecipeID = copyMe.RecipeID;
            this.ItemID = copyMe.ItemID;
            this.Quantity = copyMe.Quantity;
        }

        public override bool Equals(object? obj)
        {
            return obj is IngredientPoco poco &&
                   RecipeID == poco.RecipeID &&
                   ItemID == poco.ItemID &&
                   Quantity == poco.Quantity;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(RecipeID, ItemID, Quantity);
        }
    }
}
