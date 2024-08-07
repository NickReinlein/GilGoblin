using System.Collections.Generic;
using System.Linq;

namespace GilGoblin.Fetcher.Pocos;

public class PriceWebResponse : IResponseToList<PriceWebPoco>
{
    public Dictionary<int, PriceWebPoco> Items { get; set; }

    public PriceWebResponse(Dictionary<int, PriceWebPoco>? items)
    {
        Items = items;
    }

    public List<PriceWebPoco> GetContentAsList() => Items.Values.ToList();
}
