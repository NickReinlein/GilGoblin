using System;

namespace GilGoblin.Database.Pocos;

public abstract record CalculatedMetricPoco(
    int RecipeId,
    int WorldId,
    bool IsHq,
    int Amount,
    DateTimeOffset LastUpdated)
    : IIdentifiable
{
    public int Amount { get; set; } = Amount;
    public DateTimeOffset LastUpdated { get; set; } = LastUpdated;

    public int GetId() => RecipeId;
}