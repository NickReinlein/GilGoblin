using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GilGoblin.POCOs
{
    public class IngredientPoco
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int recipeId { get; set; }
        public int itemId { get; set; }
        public int quantity { get; set; }

        public IngredientPoco(int itemId, int quantity, int recipeId)
        {
            this.itemId = itemId;
            this.quantity = quantity;
            this.recipeId = recipeId;
        }
    }
}
