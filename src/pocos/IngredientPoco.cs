using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GilGoblin.pocos
{
    public class IngredientPoco
    {
        // Primary key would now be recipeID & ItemID
        [Key]
        public int recipeID { get; set; }
        [Key]
        public int itemID { get; set; }
        public int quantity { get; set; }

        public IngredientPoco(int itemID, int quantity, int recipeID)
        {
            this.itemID = itemID;
            this.quantity = quantity;
            this.recipeID = recipeID;
        }
    }
}
