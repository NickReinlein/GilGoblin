namespace GilGoblin.Pocos;

public abstract record BasePricePoco
{
    public int ItemID { get; set; }
    public int WorldID { get; set; }
        // The last upload time for this endpoint, in milliseconds since the UNIX epoch.
    public long LastUploadTime { get; set; }
}
