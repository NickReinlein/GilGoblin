using System;

namespace GilGoblin.Database.Pocos;

public abstract record CalculatedMetricPoco(
    int RecipeId,
    int WorldId,
    bool IsHq,
    int Amount,
    DateTime LastUpdated)
    : IIdentifiable
{
    public int GetId() => RecipeId;
}