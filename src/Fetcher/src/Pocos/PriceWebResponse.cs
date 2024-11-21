using System.Collections.Generic;
using GilGoblin.Database.Pocos;

namespace GilGoblin.Fetcher.Pocos;

public record PriceWebResponse(List<PriceWebPoco> Results, List<int> FailedItems)
    : IResponseToList<PriceWebPoco>
{
    public List<PriceWebPoco> GetContentAsList() => Results;
}