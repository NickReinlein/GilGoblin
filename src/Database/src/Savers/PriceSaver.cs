using System;
using System.Collections.Generic;
using System.Linq;
using GilGoblin.Database.Pocos;
using Microsoft.Extensions.Logging;
using EFCore.BulkExtensions;

namespace GilGoblin.Database.Savers;

public interface IPriceSaver : IDataSaver<PricePoco>;

public class PriceSaver(IServiceProvider serviceProvider, ILogger<DataSaver<PricePoco>> logger)
    : DataSaver<PricePoco>(serviceProvider, logger), IPriceSaver
{
    protected override List<PricePoco> FilterInvalidEntities(IEnumerable<PricePoco> entities)
    {
        return entities.Where(t => t is { ItemId: > 0, WorldId : > 0 }).ToList();
    }

    protected override BulkConfig GetBulkConfig()
    {
        return new BulkConfig
        {
            UpdateByProperties = ["ItemId", "WorldId", "IsHq"], // unique keys
            PreserveInsertOrder = false,
            SetOutputIdentity = true
        };
    }
}