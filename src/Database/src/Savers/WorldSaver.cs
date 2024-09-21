using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database.Savers;

public class WorldSaver(IServiceProvider serviceProvider, ILogger<DataSaver<WorldPoco>> logger)
    : DataSaver<WorldPoco>(serviceProvider, logger)
{
    protected override async Task<int> UpdateContextAsync(List<WorldPoco> worlds)
    {
        await using var scope = ServiceProvider.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        var itemIdList = worlds.Select(p => p.GetId()).ToList();
        var existing = await dbContext.World
            .Where(p => itemIdList.Contains(p.Id))
            .Select(s => s.Id)
            .ToListAsync();
        foreach (var world in worlds)
            dbContext.Entry(world).State = existing.Contains(world.Id) ? EntityState.Modified : EntityState.Added;
        return await dbContext.SaveChangesAsync();
    }
}