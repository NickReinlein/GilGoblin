using System.Collections.Generic;

namespace GilGoblin.Fetcher.Pocos;

public record PriceAggregatedWebResponse(List<PriceAggregatedWebPoco> Results, List<int> FailedItems)
    : IResponseToList<PriceAggregatedWebPoco>
{
    public List<PriceAggregatedWebPoco> GetContentAsList() => Results;
}