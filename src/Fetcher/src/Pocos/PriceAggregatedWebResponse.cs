using System.Collections.Generic;
using System.Linq;

namespace GilGoblin.Fetcher.Pocos;

public class PriceAggregatedWebResponse(Dictionary<int, PriceAggregatedWebPoco>? items)
    : IResponseToList<PriceAggregatedWebPoco>
{
    public Dictionary<int, PriceAggregatedWebPoco> Items { get; set; } = items;

    public List<PriceAggregatedWebPoco> GetContentAsList() => Items.Values.ToList();
}