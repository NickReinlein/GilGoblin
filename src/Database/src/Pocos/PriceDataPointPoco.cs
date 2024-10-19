namespace GilGoblin.Database.Pocos;

public record PriceDataPointPoco(
    int ItemId,
    bool IsHq,
    int? WorldDataPointId = null,
    int? DcDataPointId = null,
    int? RegionDataPointId = null)
    : IdentifiablePoco
{
    public PriceDataPoco? DcData { get; set; }
    public PriceDataPoco? RegionData { get; set; }
    public PriceDataPoco? WorldData { get; set; }
    
    public PriceDataPoco? GetBestPrice() => DcData ?? RegionData ?? WorldData;
    public decimal GetBestPriceCost() => GetBestPrice()?.Price ?? -1m;
}