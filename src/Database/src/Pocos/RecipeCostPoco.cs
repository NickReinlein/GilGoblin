using System;

namespace GilGoblin.Database.Pocos;

public record RecipeCostPoco(
    int RecipeId,
    int WorldId,
    bool IsHq,
    int Amount,
    DateTimeOffset LastUpdated)
    : CalculatedMetricPoco(RecipeId, WorldId, IsHq, Amount, LastUpdated);