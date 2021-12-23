using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GilGoblin.Finance
{
    public class Recipe
    {
        public int recipe_id { get; set; }
        public int target_item_id { get; set; }
        public int icon_id { get; set; }
        public int result_quantity { get; set; }
        public bool CanHq { get; set; }
        public bool CanQuickSynth { get; set; }

        public Recipe() { }
    }

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
