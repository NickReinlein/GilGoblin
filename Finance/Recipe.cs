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
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        public int recipe_id { get; set; }

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [ForeignKey("itemID")]
        [InverseProperty("ItemDB")]
        public int target_item_id { get; set; }
        public int icon_id { get; set; }
        public int result_quantity { get; set; }
        public bool CanHq { get; set; }
        public bool CanQuickSynth { get; set; }
        public ICollection<Ingredient> ingredients { get; set; }
            = new List<Ingredient>();

        public Recipe() { }
    }
}
