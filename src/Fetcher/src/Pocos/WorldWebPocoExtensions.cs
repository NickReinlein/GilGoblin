using System.Collections.Generic;
using System.Linq;
using GilGoblin.Database.Pocos;

namespace GilGoblin.Fetcher.Pocos;

public static class WorldWebPocoExtensions
{
    public static WorldPoco ToWorldPoco(this WorldWebPoco webPoco) =>
        new() { Id = webPoco.GetId(), Name = webPoco.Name };

    public static List<WorldPoco> ToDatabasePoco(this IEnumerable<WorldWebPoco?> pocos) =>
        pocos.Where(poco => poco is not null).Select(world => world!.ToWorldPoco()).ToList();
}