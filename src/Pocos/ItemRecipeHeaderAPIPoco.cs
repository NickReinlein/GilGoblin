using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GilGoblin.Pocos
{
    public class ItemRecipeHeaderPoco
    {
        public int ID { get; set; }
        public int classJobID { get; set; }
        public int recipeID { get; set; }
        public int level { get; set; }

        public ItemRecipeHeaderPoco() { }

        [JsonConstructor]
        public ItemRecipeHeaderPoco(int ID, int classJobID, int level)
        {
            this.classJobID = classJobID;
            this.recipeID = ID;
            this.level = level;
        }

    }
}
