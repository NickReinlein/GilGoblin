namespace GilGoblin.Pocos;

public class BasePricePoco
{
    public int WorldID { get; set; }
    public int ItemID { get; set; }

    // The last upload time for this endpoint, in milliseconds since the UNIX epoch.
    public long LastUploadTime { get; set; }
}
