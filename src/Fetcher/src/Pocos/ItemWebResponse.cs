using System.Collections.Generic;
using System.Linq;
using GilGoblin.Database.Pocos;

namespace GilGoblin.Fetcher.Pocos;

public class ItemWebResponse : IResponseToList<ItemWebPoco>
{
    public Dictionary<int, ItemWebPoco> Items { get; set; }

    public ItemWebResponse(Dictionary<int, ItemWebPoco> items)
    {
        Items = items;
    }

    public ItemWebResponse(IEnumerable<ItemPoco> dbItems)
    {
        Items = new Dictionary<int, ItemWebPoco>();
        foreach (var item in dbItems)
        {
            var converted = new ItemWebPoco(item);
            Items.Add(converted.Id, converted);
        }
    }

    public List<ItemWebPoco> GetContentAsList() => Items?.Values.ToList();
}