using System.Collections.Generic;

namespace GilGoblin.Fetcher.Pocos;

public record PriceWebResponse(List<PriceWebPoco> Results, List<int> FailedItems)
    : IResponseToList<PriceWebPoco>
{
    public List<PriceWebPoco> GetContentAsList() => Results;
}