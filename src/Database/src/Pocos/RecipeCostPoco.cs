using System;

namespace GilGoblin.Database.Pocos;

public record RecipeCostPoco : IIdentifiable
{
    public int RecipeId { get; init; }
    public int WorldId { get; init; }
    public bool IsHq { get; init; }
    public int Cost { get; set; }
    public DateTime LastUpdated { get; set; }
    public int GetId() => RecipeId;
}