using System;
using System.Collections.Generic;
using System.Linq;
using GilGoblin.Crafting;
using GilGoblin.Ext;
using GilGoblin.Pocos;
using GilGoblin.Utility;
using GilGoblin.Web;
using Microsoft.Extensions.Logging;
using Serilog;

namespace GilGoblin.Database
{
    public class MarketDataDB : MarketDataPoco, IMarketDataDB
    {
        private readonly ILogger<MarketDataDB> _log;
        public IEnumerable<MarketDataPoco> GetMarketData(int worldId, IEnumerable<int> itemIDs)
        {
            var marketData = Array.Empty<MarketDataPoco>();
            try
            {
                if (worldId == 0 || !itemIDs.Any())
                    throw new Exception("Error with parameters");


            }
            catch (Exception ex)
            {
                _log.LogError("Failed to get market data from database: {Message}", ex.Message);
                return Array.Empty<MarketDataPoco>();
            }
            return marketData;
        }

        public MarketDataDB(int itemID, int worldID, long lastUploadTime, string? name, string? regionName, float currentAveragePrice, float currentAveragePriceNQ, float currentAveragePriceHQ, float averagePrice, float averagePriceNQ, float averagePriceHQ) : base(itemID, worldID, lastUploadTime, name, regionName, currentAveragePrice, currentAveragePriceNQ, currentAveragePriceHQ, averagePrice, averagePriceNQ, averagePriceHQ)
        {
        }

        public MarketDataDB(MarketDataPoco copyMe) : base(copyMe) { }
    }
}
