namespace GilGoblin.Database.Pocos;

public record PriceDataDbPoco(int Id, string PriceType, float Price, long Timestamp, int WorldId);