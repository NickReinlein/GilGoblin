using System.Collections.Generic;
using System.Linq;
using GilGoblin.Pocos;

namespace GilGoblin.Web;

public class ItemInfoWebResponse : IReponseToList<ItemInfoWebPoco>
{
    public Dictionary<int, ItemInfoWebPoco> Items { get; set; }

    public ItemInfoWebResponse(Dictionary<int, ItemInfoWebPoco> items)
    {
        Items = items ?? new Dictionary<int, ItemInfoWebPoco>();
    }

    public List<ItemInfoWebPoco> GetContentAsList() => Items?.Values?.ToList();
}
