namespace GilGoblin.Database.Pocos;

public record PriceDataPoco(int Id, string PriceType, decimal Price, int? WorldId, long? Timestamp)
: PriceDataDetailPoco(Price, WorldId, Timestamp);