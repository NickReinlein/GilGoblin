namespace GilGoblin.Database.Pocos;

public record PriceDataPoco(int Id, string PriceType, float Price, long Timestamp, int WorldId);