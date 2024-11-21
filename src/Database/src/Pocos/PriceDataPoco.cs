using System;

namespace GilGoblin.Database.Pocos;

public record PriceDataPoco(
    string PriceType,
    decimal Price,
    int WorldId,
    long? Timestamp = null)
    : IdentifiablePoco
{
    public decimal Price { get; set; } = Price;
    public long? Timestamp { get; set; } = Timestamp ?? DateTimeOffset.UtcNow.Ticks;
}