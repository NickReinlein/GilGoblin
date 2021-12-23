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
        public int itemID { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int iconID { get; set; }
        public int vendor_price { get; set; }
        public int stack_size { get; set; }
        public int gatheringID { get; set; }

        public ItemInfo() { }
    }    
}
