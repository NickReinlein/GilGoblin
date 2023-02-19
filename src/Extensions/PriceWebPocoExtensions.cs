using System.Collections.Generic;
using GilGoblin.Pocos;

namespace GilGoblin.Extensions;

public static class PriceWebPocoExtensions
{
    public static PricePoco ToPricePoco(this PriceWebPoco poco) => new(poco);

    public static List<PricePoco> ToPricePocoList(this IEnumerable<PriceWebPoco?> pocos)
    {
        // Is there a fancy LINQ we could do ehre?
        var list = new List<PricePoco>();
        foreach (var poco in pocos)
        {
            if (poco is null)
                continue;

            list.Add(new PricePoco(poco));
        }
        return list;
    }
}
