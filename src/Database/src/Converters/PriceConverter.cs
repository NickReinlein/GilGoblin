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
    (PricePoco?, PricePoco?) Convert(PriceWebPoco webPoco, int worldId);
}

public class PriceConverter(GilGoblinDbContext dbContext, ILogger<PriceConverter> logger) : IPriceConverter
{
    public (PricePoco?, PricePoco?) Convert(PriceWebPoco webPoco, int worldId)
    {
        if (worldId == 0)
            return (null, null);

        try
        {
            var (hq, nq) = GetPricePocoPerQualityAsync(webPoco, worldId);
            return (hq, nq);
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "Failed to convert price data");
            return (null, null);
        }
    }

    private (PricePoco?, PricePoco?) GetPricePocoPerQualityAsync(PriceWebPoco webPoco, int worldId)
    {
        try
        {
            // take the web poco 
            // take hq and nq separately as QualityPriceDataPoco
            // take 3 price types from each of 2 QualityPriceDataPoco as PriceDataPointWebPoco
            // take 3 regions from each PriceDataPointWebPoco as PriceDataDetailPoco
            // save each region data point
            // take the 3 ids returned and assign them to a new PriceDataPointPoco
            // do this for 3 price types into a new PricePoco
            // do this for hq and nq for 2 new pricePocos
            
            // PriceWebPoco (includes hq and nq)
            // 2 QualityPriceDataPoco >> (2 PricePoco later)
            // 3 PriceDataPointWebPoco >> PriceDataPointPoco
            // 1 PricePoco 
            // => 2 PricePocos total, hq + nq
            
            var hq = new PricePoco()
            {
                Id = 0,
                WorldId = worldId,
                ItemId = webPoco.ItemId,
                IsHq = true,
                Updated = DateTimeOffset.Now,
                AverageSalePrice = new AverageSalePricePoco(0, webPoco.ItemId, true)
            };
            return (hq, null);
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "Failed to get quality price data from web poco");
            return (null, null);
        }
    }
}