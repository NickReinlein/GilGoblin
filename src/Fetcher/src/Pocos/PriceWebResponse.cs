using System.Collections.Generic;

namespace GilGoblin.Fetcher.Pocos;

public record PriceWebResponse(List<PricePoco> Results, List<int> FailedItems)
    : IResponseToList<PricePoco>
{
    public List<PricePoco> GetContentAsList() => Results;
}