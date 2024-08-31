namespace GilGoblin.Database.Pocos;

public class PriceDataPointsPoco
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public decimal Price { get; set; }
    public int? WorldId { get; set; }
    public int? DcId { get; set; }
    public int? RegionId { get; set; }
    public long Timestamp { get; set; }
}