namespace GilGoblin.Database.Pocos;

public record PriceDataDbPoco(int Id, string PriceType, float Price, long Timestamp, int WorldId)
{
    public int Id { get; set; }
    public string PriceType { get; set; }
    public float Price { get; set; }
    public long Timestamp { get; set; }
    public int WorldId { get; set; }
}