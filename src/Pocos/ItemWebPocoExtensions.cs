using System.Collections.Generic;
using System.Linq;
using GilGoblin.Database.Pocos;

namespace GilGoblin.Pocos;

public static class ItemWebPocoExtensions
{
    public static ItemPoco ToItemPoco(this ItemWebPoco webPoco) =>
        new()
        {
            Id = webPoco.Id,
            Description = webPoco.Description,
            Level = webPoco.Level,
            Name = webPoco.Name,
            CanHq = webPoco.CanHq,
            IconId = webPoco.IconId,
            PriceMid = webPoco.PriceMid,
            PriceLow = webPoco.PriceLow,
            StackSize = webPoco.StackSize
        };

    public static List<ItemPoco> ToItemPocoList(this IEnumerable<ItemWebPoco?> pocos) =>
        pocos.Where(poco => poco is not null).Select(item => item.ToItemPoco()).ToList();
}