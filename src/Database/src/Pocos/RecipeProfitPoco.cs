using System;

namespace GilGoblin.Database.Pocos;

public class RecipeProfitPoco
{
    public int RecipeId { get; set; }
    public int WorldId { get; set; }
    public int RecipeProfitVsSold { get; set; }
    public int RecipeProfitVsListings { get; set; }
    public DateTimeOffset Updated { get; set; }
}