using GilGoblin.Pocos;

namespace GilGoblin.Web;

public class PriceWebResponse : IReponseToList<PriceWebPoco>
{
    public Dictionary<int, PriceWebPoco> Items { get; set; }

    public PriceWebResponse(Dictionary<int, PriceWebPoco> items)
    {
        Items = items;
    }

    public List<PriceWebPoco> GetContentAsList()
    {
        return Items.Values.ToList();
    }
}
