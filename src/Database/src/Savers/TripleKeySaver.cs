using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database.Savers;

public class TripleKeySaver<T>(IServiceProvider serviceProvider, ILogger<DataSaver<T>> logger)
    : DataSaver<T>(serviceProvider, logger)
    where T : IdentifiableTripleKeyPoco
{
    protected override async Task<int> UpdateContextAsync(List<T> entityList, CancellationToken ct = default)
    {
        try
        {
            await using var scope = ServiceProvider.CreateAsyncScope();
            await using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();

            var toUpdate = await dbContext
                .Set<T>()
                .Where(sale => entityList.Any(e => e.GetKey().Equals(sale.GetKey())))
                .ToListAsync(cancellationToken: ct);
            var toAdd = entityList.Except(toUpdate).ToList();

            if (toAdd.Any())
                await dbContext.Set<T>().AddRangeAsync(toAdd, ct);

            if (toUpdate.Any())
                dbContext.Set<T>().UpdateRange(toUpdate);

            return await dbContext.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update due to error");
            return 0;
        }
    }
}