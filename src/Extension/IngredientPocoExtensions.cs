using GilGoblin.Pocos;

namespace GilGoblin.Extensions;

public static class IngredientPocoExtensions
{
    public static int GetIngredientsSum(this IEnumerable<IngredientPoco?> pocos)
    {
        return pocos.Where(p => p is not null && p.Quantity > 0).Sum(s => s!.Quantity);
    }

    public static int GetIngredientsCount(this IEnumerable<IngredientPoco?> pocos)
    {
        return pocos.Count(p => p is not null && p.Quantity > 0);
    }
}
