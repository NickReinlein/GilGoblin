using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using GilGoblin.Database.Pocos.Extensions;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database.Converters;

public interface IPriceConverter
{
    Task<(PricePoco?, PricePoco?)> ConvertAsync(PriceWebPoco webPoco, int worldId);
}

public class PriceConverter(GilGoblinDbContext dbContext, IPriceDataPointConverter dataPointConverter, ILogger<PriceConverter> logger) : IPriceConverter
{
    public async Task<(PricePoco?, PricePoco?)> ConvertAsync(PriceWebPoco webPoco, int worldId)
    {
        try
        {
            // 1. take the web poco 
            // 2. take hq and nq separately as QualityPriceDataPoco
            // 3. take 3 price types from each of 2 QualityPriceDataPoco as PriceDataPointWebPoco
            // 4. take 3 regions from each PriceDataPointWebPoco as PriceDataDetailPoco
            // 5. save each region data point
            // 6. take the 3 ids returned and assign them to a new PriceDataPointPoco
            // 7. do this for 3 price types into a new PricePoco
            // 8. do this for hq and nq for 2 new pricePocos
            
            // PriceWebPoco (includes hq and nq)
            // 2 QualityPriceDataPoco >> (2 PricePoco later)
            // 3 PriceDataPointWebPoco >> 
            // 1 PricePoco 
            // => 2 PricePocos total, hq + nq

            if (worldId == 0)
                throw new ArgumentException("Invalid world id", nameof(worldId));
            
            var itemId = webPoco.ItemId;
            var hq = await dataPointConverter.ConvertAsync(webPoco.Hq, itemId, true);
            var nq = webPoco.Nq;
            var uploadTimes = webPoco.WorldUploadTimes;
            

            return (null, null);
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "Failed to convert price data");
            return (null, null);
        }
    }
}