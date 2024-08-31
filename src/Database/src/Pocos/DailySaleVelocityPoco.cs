namespace GilGoblin.Database.Pocos;

public record DailySaleVelocityPoco
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public decimal? WorldQuantity { get; set; }
    public decimal? DcQuantity { get; set; }
    public decimal? RegionQuantity { get; set; }
}