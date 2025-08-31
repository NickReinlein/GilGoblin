namespace GilGoblin.Database.Pocos;

public record PriceDataPoco(
    string PriceType,
    decimal Price,
    int WorldId,
    long? Timestamp = null)
    : IdentifiablePoco;