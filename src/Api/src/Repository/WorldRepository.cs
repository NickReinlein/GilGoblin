using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Api.Cache;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;
using Microsoft.Extensions.DependencyInjection;

namespace GilGoblin.Api.Repository;

public class WorldRepository(IServiceProvider serviceProvider, IWorldCache cache) : IWorldRepository
{
    public WorldPoco? Get(int id)
    {
        var cached = cache.Get(id);
        if (cached is not null)
            return cached;

        using var scope = serviceProvider.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        var world = dbContext.World.FirstOrDefault(i => i.Id == id);
        if (world is not null)
            cache.Add(world.GetId(), world);
        return world;
    }

    public List<WorldPoco> GetMultiple(IEnumerable<int> ids)
    {
        using var scope = serviceProvider.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        return dbContext.World.Where(w => ids.Contains(w.Id ?? 0)).ToList();
    }

    public List<WorldPoco> GetAll()
    {
        using var scope = serviceProvider.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        return dbContext.World.ToList();
    }

    public async Task FillCache()
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        var worlds = dbContext.World.ToList();
        worlds.ForEach(world => cache.Add(world.GetId(), world));
    }
}