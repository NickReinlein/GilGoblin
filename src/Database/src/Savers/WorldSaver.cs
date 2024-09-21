using System;
using GilGoblin.Database.Pocos;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database.Savers;

public class WorldSaver(IServiceProvider serviceProvider, ILogger<DataSaver<WorldPoco>> logger)
    : DataSaver<WorldPoco>(serviceProvider, logger);