namespace GilGoblin.Database.Pocos;

using System;

public record RecipeProfitPoco
{
    public int RecipeId { get; init; }
    public int WorldId { get; init; }
    public bool IsHq { get; init; }
    public int Profit { get; set; }
    public DateTime LastUpdated { get; set; }
}