using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public ItemRecipeHeaderPoco(int ID, int classJobID, int level)
        {
            this.ClassJobID = classJobID;
            this.RecipeID = ID;
            this.Level = level;
        }

    }
}
