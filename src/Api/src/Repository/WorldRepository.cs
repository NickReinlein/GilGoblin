using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Api.Cache;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;

namespace GilGoblin.Api.Repository;

public class WorldRepository : IWorldRepository
{
    private readonly GilGoblinDbContext _dbContext;
    private readonly IWorldCache _cache;

    public WorldRepository(GilGoblinDbContext dbContext, IWorldCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    public WorldPoco? Get(int id)
    {
        var cached = _cache.Get(id);
        if (cached is not null)
            return cached;

        var world = _dbContext?.World?.FirstOrDefault(i => i.Id == id);
        if (world is not null)
            _cache.Add(world.Id, world);

        return world;
    }

    public IEnumerable<WorldPoco> GetMultiple(IEnumerable<int> ids) =>
        _dbContext?.World?.Where(w => ids.Contains(w.Id)).AsEnumerable();

    public IEnumerable<WorldPoco> GetAll() => _dbContext?.World.AsEnumerable();

    public Task FillCache()
    {
        var worlds = _dbContext?.World?.ToList();
        worlds?.ForEach(world => _cache.Add(world.Id, world));
        return Task.CompletedTask;
    }
}