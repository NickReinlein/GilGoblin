using System.Collections.Generic;
using System.Linq;

namespace GilGoblin.Fetcher.Pocos;

public class PriceWebResponse(Dictionary<int, PriceWebPoco>? items) : IResponseToList<PriceWebPoco>
{
    public Dictionary<int, PriceWebPoco> Items { get; set; } = items;

    public List<PriceWebPoco> GetContentAsList() => Items.Values.ToList();
}
