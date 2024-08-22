using System.Collections.Generic;
using System.Linq;

namespace GilGoblin.Fetcher.Pocos;

public class PriceAggregatedWebResponse(Dictionary<int, PriceAggregatedWebPoco>? items = null)
    : IResponseToList<PriceAggregatedWebPoco>
{
    public List<PriceAggregatedWebPoco> GetContentAsList() => items?.Values.ToList() ?? [];
}