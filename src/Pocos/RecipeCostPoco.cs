using System;

namespace GilGoblin.Pocos;

public class RecipeCostPoco
{
    public int RecipeID { get; set; }
    public int WorldID { get; set; }
    public int Cost { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset Updated { get; set; }
}
