using System.Collections.Generic;
using System.Linq;
using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database.Savers;

public class WorldSaver : DataSaver<WorldPoco>
{
    public WorldSaver(GilGoblinDbContext context, ILogger<DataSaver<WorldPoco>> logger) : base(context, logger)
    {
    }

    protected override void UpdateContext(List<WorldPoco> worlds)
    {
        var itemIdList = worlds.Select(p => p.GetId()).ToList();
        var existing = Context.World
            .Where(p => itemIdList.Contains(p.Id))
            .Select(s => s.Id)
            .ToList();
        foreach (var world in worlds)
            Context.Entry(world).State = existing.Contains(world.Id) ? EntityState.Modified : EntityState.Added;
    }
}