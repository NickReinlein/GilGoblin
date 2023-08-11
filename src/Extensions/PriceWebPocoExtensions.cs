using System.Collections.Generic;
using System.Linq;
using GilGoblin.Pocos;

namespace GilGoblin.Extensions;

public static class PriceWebPocoExtensions
{
    public static PricePoco ToPricePoco(this PriceWebPoco poco) => new(poco);

    public static List<PricePoco> ToPricePocoList(this IEnumerable<PriceWebPoco?> pocos) =>
        pocos.Where(poco => poco is not null).Select(price => price.ToPricePoco()).ToList();

    // var list = new List<PricePoco>();
    // foreach (var poco in pocos)
    // {
    //     if (poco is null)
    //         continue;

    //     list.Add(new PricePoco(poco));
    // }
    // return list;
}
