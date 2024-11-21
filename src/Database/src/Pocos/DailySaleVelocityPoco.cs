namespace GilGoblin.Database.Pocos;

public record DailySaleVelocityPoco(
    int ItemId,
    int WorldId,
    bool IsHq,
    decimal? World = null,
    decimal? Dc = null,
    decimal? Region = null)
    : IdentifiableTripleKeyPoco(ItemId, WorldId, IsHq)
{
    public decimal? World { get; set; } = World;
    public decimal? Dc { get; set; } = Dc;
    public decimal? Region { get; set; } = Region;
}