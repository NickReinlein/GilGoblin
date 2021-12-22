using Newtonsoft.Json;

namespace GilGoblin.WebAPI
{
    internal class APIRecipe
    {
        public int class_job_id { get; set; }
        public int item_id { get; set; }
        public int level { get; set; }

        [JsonConstructor]
        public APIRecipe(int id, int classJobID, int level)
        {
            this.class_job_id = classJobID;
            this.item_id = id;
            this.level = level;
        }

    }
}
