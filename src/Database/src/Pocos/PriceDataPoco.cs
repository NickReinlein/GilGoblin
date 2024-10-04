namespace GilGoblin.Database.Pocos;

public record PriceDataPoco(string PriceType, decimal Price, int? WorldId = null, long? Timestamp = null)
    : IdentifiablePoco;