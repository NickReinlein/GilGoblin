using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database
{
    public class DataSaver<T> : IDataSaver<T> where T : class, IIdentifiable
    {
        protected readonly GilGoblinDbContext Context;
        private readonly ILogger<DataSaver<T>> _logger;

        public DataSaver(GilGoblinDbContext context, ILogger<DataSaver<T>> logger)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> SaveAsync(IEnumerable<T> entities)
        {
            var entityList = entities.ToList();
            if (!entityList.Any())
                return false;

            try
            {
                ValidateEntities(entityList);

                UpdateContext(entityList);

                var savedCount = await Context.SaveChangesAsync();
                _logger.LogInformation($"Saved {savedCount} new entries for type {typeof(T).Name}");

                var failedCount = entityList.Count - savedCount;
                if (failedCount > 0)
                {
                    throw new InvalidOperationException($"Failed to save {failedCount} entities!");
                }

                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Failed to update due to database error: {ex.Message}");
                return false;
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, $"Failed to update due to invalid data: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to update due to unknown error: {ex.Message}");
                return false;
            }
        }

        protected virtual void UpdateContext(List<T> entityList)
        {
            foreach (var entity in entityList)
            {
                Context.Entry(entity).State = entity.GetId() == 0 ? EntityState.Added : EntityState.Modified;
            }
        }

        protected virtual void ValidateEntities(IEnumerable<T> entities)
        {
            if (entities.Any(t => t.GetId() < 0))
            {
                throw new ArgumentException("Cannot save entities due to error in key field");
            }
        }
    }
}