using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database.Savers;

public class TripleKeySaver<T>(IServiceProvider serviceProvider, ILogger<TripleKeySaver<T>> logger)
    : DataSaver<T>(serviceProvider, logger)
    where T : IdentifiableTripleKeyPoco
{
    protected override async Task<int> SaveToDatabaseAsync(List<T> entityList, CancellationToken ct = default)
    {
        try
        {
            await using var scope = ServiceProvider.CreateAsyncScope();
            await using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();

            var toUpdate = new List<T>();
            var toAdd = new List<T>();
            foreach (var entity in entityList)
            {
                var existingKeys = await dbContext
                    .Set<T>()
                    .FirstOrDefaultAsync(e =>
                        e.ItemId == entity.ItemId &&
                        e.WorldId == entity.WorldId &&
                        e.IsHq == entity.IsHq, ct);
                if (existingKeys is not null)
                    toUpdate.Add(existingKeys);
                else
                    toAdd.Add(entity);
            }

            dbContext.UpdateRange(toUpdate);
            dbContext.AddRange(toAdd);

            return await dbContext.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update due to error");
            return 0;
        }
    }
}