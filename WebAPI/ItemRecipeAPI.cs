using Newtonsoft.Json;

namespace GilGoblin.WebAPI
{
    internal class ItemRecipeAPI
    {
        public int class_job_id { get; set; }
        public int recipe_id { get; set; }
        public int level { get; set; }

        [JsonConstructor]
        public ItemRecipeAPI(int id, int classJobID, int level)
        {
            this.class_job_id = classJobID;
            this.recipe_id = id;
            this.level = level;
        }

    }
}
