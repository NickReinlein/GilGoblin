using System;

namespace GilGoblin.Database.Pocos;

public class RecipeCostPoco
{
    public int RecipeId { get; set; }
    public int WorldId { get; set; }
    public int Cost { get; set; }

    public DateTimeOffset Created { get; set; }
    public DateTimeOffset Updated { get; set; }
}