using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace GilGoblin.Pocos
{
    public class ItemRecipeHeaderPoco
    {
        public int ID { get; set; }
        public int ClassJobID { get; set; }
        public int RecipeID { get; set; }
        public int Level { get; set; }

        public ItemRecipeHeaderPoco() { }

        [JsonConstructor]
        public ItemRecipeHeaderPoco(int id, int classJobID, int level)
        {
            this.ClassJobID = classJobID;
            this.RecipeID = id;
            this.Level = level;
        }

    }
}
