using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GilGoblin.WebAPI
{
    internal class ItemInfo
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [ForeignKey("itemId")]
        [InverseProperty("ItemDB")]
        public int item_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int icon_id { get; set; }
        public int vendor_price { get; set; }
        public int stack_size { get; set; }
        public int gathering_id { get; set; }

        public ItemInfo() { }
    }    
}
