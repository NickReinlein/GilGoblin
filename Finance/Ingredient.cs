using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GilGoblin.Finance
{
    public class Ingredient
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int recipeID { get; set; }
        public int itemID { get; set; }
        public int quantity { get; set; }

        public Ingredient(int itemID, int quantity, int recipeID)
        {
            this.itemID = itemID;
            this.quantity = quantity;
            this.recipeID = recipeID;
        }
    }
}
