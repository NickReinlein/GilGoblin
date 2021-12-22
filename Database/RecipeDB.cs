using GilGoblin.Finance;
using GilGoblin.WebAPI;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GilGoblin.Database
{
    public class RecipeDB : Recipe
    {
        public ICollection<Ingredient> ingredients { get; set; } = new List<Ingredient>();

        public RecipeDB() : base() { }
        public RecipeDB(RecipeWeb web) : base()
        {
            icon_id = web.icon_id;
            CanHq = web.CanHq;
            CanQuickSynth = web.CanQuickSynth;
            target_item_id = web.target_item_id;
            recipe_id = web.recipe_id;
            result_quantity = web.result_quantity;

            foreach (KeyValuePair<int, int> ingredient in web.ingredients)
            {
                if (ingredient.Value == 0) { continue; }
                ingredients.Add(new Ingredient(ingredient.Key, ingredient.Value));

            }
        }

        public class Ingredient
        {
            [Key]
            [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
            public int Id { get; set; }

            [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
            [ForeignKey("item_id")]
            [InverseProperty("MarketDataDB")]
            public int item_id { get; set; }
            public int quantity { get; set; }

            public Ingredient(int item_id, int quantity)
            {
                this.item_id = item_id;
                this.quantity = quantity;
            }
        }
    }
}
