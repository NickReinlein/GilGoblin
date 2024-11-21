namespace GilGoblin.Database.Pocos;

public record BaseItemPoco : IdentifiablePoco
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int IconId { get; set; }
    public int Level { get; set; }
    public int StackSize { get; set; }
    public int PriceMid { get; set; }
    public int PriceLow { get; set; }
    public bool CanHq { get; set; }

    public BaseItemPoco() { }

    protected BaseItemPoco(
        int id,
        string name,
        string description,
        int iconId,
        int priceMid,
        int priceLow,
        int stackSize,
        int level,
        bool canHq)
    {
        Id = id;
        Name = name;
        Description = description;
        IconId = iconId;
        Level = level;
        PriceMid = priceMid;
        PriceLow = priceLow;
        StackSize = stackSize;
        CanHq = canHq;
    }
}