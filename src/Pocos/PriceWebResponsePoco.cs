using GilGoblin.Pocos;

namespace GilGoblin.Web;

public class PriceWebResponsePoco : IReponseToList<PriceWebPoco>
{
    public Dictionary<int, PriceWebPoco> Items { get; set; }

    public PriceWebResponsePoco(Dictionary<int, PriceWebPoco> items)
    {
        Items = items;
    }

    public List<PriceWebPoco> GetContentAsList()
    {
        return Items.Values.ToList();
    }
}
