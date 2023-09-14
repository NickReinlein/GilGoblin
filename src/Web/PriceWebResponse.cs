using System.Collections.Generic;
using System.Linq;
using GilGoblin.Pocos;

namespace GilGoblin.Web;

public class PriceWebResponse : IReponseToList<PriceWebPoco>
{
    public Dictionary<int, PriceWebPoco> Items { get; set; }

    public PriceWebResponse(Dictionary<int, PriceWebPoco> items)
    {
        Items = items ?? new Dictionary<int, PriceWebPoco>();
    }

    public List<PriceWebPoco> GetContentAsList() => Items?.Values?.ToList();
}
