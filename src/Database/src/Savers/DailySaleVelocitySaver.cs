using System;
using GilGoblin.Database.Pocos;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database.Savers;

public class DailySaleVelocitySaver(IServiceProvider serviceProvider, ILogger<DailySaleVelocitySaver> logger)
    : TripleKeySaver<DailySaleVelocityPoco>(serviceProvider, logger);