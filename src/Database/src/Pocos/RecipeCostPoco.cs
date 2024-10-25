using System;

namespace GilGoblin.Database.Pocos;

public record RecipeCostPoco(
    int RecipeId,
    int WorldId,
    bool IsHq,
    int Amount,
    DateTime LastUpdated)
    : CalculatedMetricPoco(RecipeId, WorldId, IsHq, Amount, LastUpdated);