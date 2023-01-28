using System.Text.Json.Serialization;
using Serilog;

namespace GilGoblin.Pocos;

public class ItemInfoPoco
{
    public int ID { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public int IconID { get; set; }
    public int VendorPrice { get; set; }
    public int StackSize { get; set; }

    [JsonConstructor]
    public ItemInfoPoco(
        int id,
        string name,
        string description,
        int iconID,
        int priceMid,
        int stackSize
    )
    {
        this.ID = id;
        this.IconID = iconID;
        this.Description = description;
        this.Name = name;
        this.VendorPrice = priceMid;
        this.StackSize = stackSize;
    }

    public ItemInfoPoco() { }
}
