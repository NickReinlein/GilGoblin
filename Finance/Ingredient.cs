using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GilGoblin.Finance
{
    public class Ingredient
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int item_id { get; set; }
        public int quantity { get; set; }

        public Ingredient(int item_id, int quantity)
        {
            this.item_id = item_id;
            this.quantity = quantity;
        }
    }
}
