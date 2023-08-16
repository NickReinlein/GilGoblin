using System.Collections.Generic;
using System.Linq;

namespace GilGoblin.Pocos;

public static class IngredientPocoExtensions
{
    public static int GetIngredientsSum(this IEnumerable<IngredientPoco?> pocos) =>
        pocos.Where(p => p is not null && p.Quantity > 0).Sum(s => s!.Quantity);

    public static int GetIngredientsCount(this IEnumerable<IngredientPoco?> pocos) =>
        pocos.Count(p => p is not null && p.Quantity > 0);
}
