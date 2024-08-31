namespace GilGoblin.Database.Pocos;

public class RecentPurchasePoco
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public int? WorldDataPointId { get; set; }
    public int? DcDataPointId { get; set; }
    public int? RegionDataPointId { get; set; }
}