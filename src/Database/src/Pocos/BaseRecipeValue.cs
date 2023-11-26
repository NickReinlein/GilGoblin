using System;

namespace GilGoblin.Database.Pocos;

public class BaseRecipeValue : IIdentifiable
{
    public int RecipeId { get; set; }
    public int WorldId { get; set; }
    public DateTimeOffset Updated { get; set; }
    public int GetId() => RecipeId;
}