using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GilGoblin.WebAPI
{
    internal class ItemRecipeAPI
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int class_job_id { get; set; }
        public int recipe_id { get; set; }
        public int level { get; set; }

        public ItemRecipeAPI() { }

        [JsonConstructor]
        public ItemRecipeAPI(int ID, int classJobID, int level)
        {
            this.class_job_id = classJobID;
            this.recipe_id = ID;
            this.level = level;
        }

    }
}
