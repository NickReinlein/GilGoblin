using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GilGoblin.Finance;
using GilGoblin.WebAPI;

namespace GilGoblin.Database
{
    internal class MarketDataDB
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int item_id { get; }
        public string world_name { get; }
        public DateTime last_updated { get; set; }
        public int average_Price { get; set; }

        public MarketDataDB()
        {

        }
        public MarketDataDB(MarketData marketData)
        {
            this.item_id = marketData.item_id;
            this.world_name = marketData.world_name;
            this.last_updated = marketData.last_updated;            
            this.average_Price = marketData.average_Price;
        }
    }
}
