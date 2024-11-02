namespace GilGoblin.Database.Pocos;

public record PriceDataPointPoco(
    int ItemId,
    int WorldId,
    bool IsHq,
    int? WorldDataPointId = null,
    int? DcDataPointId = null,
    int? RegionDataPointId = null)
    : IdentifiableTripleKeyPoco(ItemId, WorldId, IsHq)
{
    // Navigation properties
    public PriceDataPoco? DcDataPoint { get; set; }
    public PriceDataPoco? RegionDataPoint { get; set; }
    public PriceDataPoco? WorldDataPoint { get; set; }
    
    public PriceDataPoco? GetBestPrice() => DcDataPoint ?? RegionDataPoint ?? WorldDataPoint;
}