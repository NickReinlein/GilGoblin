using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GilGoblin.WebAPI
{
    public class ItemRecipeHeaderAPI
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int class_job_id { get; set; }
        public int recipe_id { get; set; }
        public int level { get; set; }

        public ItemRecipeHeaderAPI() { }

        [JsonConstructor]
        public ItemRecipeHeaderAPI(int ID, int classJobID, int level)
        {
            this.class_job_id = classJobID;
            this.recipe_id = ID;
            this.level = level;
        }

    }
}
