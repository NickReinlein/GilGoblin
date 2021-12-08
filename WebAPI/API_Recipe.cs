using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GilGoblin.WebAPI
{
    internal class API_Recipe
    {
        public int class_job_id { get; set; }
        public int item_id { get; set; }
        public int level { get; set; }

        public API_Recipe(int id, int classJobID, int level)
        {
            this.class_job_id = classJobID;
            this.item_id = id;
            this.level = level;
        }
            
    }
}
