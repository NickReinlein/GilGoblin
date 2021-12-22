using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GilGoblin.Finance
{
    public class Recipe
    {
        [Key]
        public int recipe_id { get; set; }
        public int target_item_id { get; set; }
        public int icon_id { get; set; }
        public int result_quantity { get; set; }
        public bool CanHq { get; set; }
        public bool CanQuickSynth { get; set; }

        public Recipe() { }
    }
}
