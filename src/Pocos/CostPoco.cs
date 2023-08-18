using System;

namespace GilGoblin.Pocos;

public class CostPoco
{
    public (int, int) Key { get; set; }
    public int Cost { get; set; }
    public int RecipeID { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset Updated { get; set; }
}
