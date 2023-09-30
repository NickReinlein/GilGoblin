using System.Collections.Generic;
using System.Linq;
using GilGoblin.Pocos;

namespace GilGoblin.Web;

public class ItemInfoWebResponse : IResponseToList<ItemInfoWebPoco>
{
    public Dictionary<int, ItemInfoWebPoco> Items { get; set; }

    public ItemInfoWebResponse(Dictionary<int, ItemInfoWebPoco> items)
    {
        Items = items ?? new Dictionary<int, ItemInfoWebPoco>();
    }

    public ItemInfoWebResponse(IEnumerable<ItemInfoPoco> items)
    {
        Items = new Dictionary<int, ItemInfoWebPoco>();
        foreach (var item in items)
        {
            var converted = new ItemInfoWebPoco(item);
            Items.Add(converted.ID, converted);
        }
    }

    public List<ItemInfoWebPoco> GetContentAsList() => Items?.Values.ToList();
}