namespace GilGoblin.Pocos;

public class PricePoco : BasePricePoco
{
    public float AverageListingPrice { get; set; }
    public float AverageListingPriceNQ { get; set; }
    public float AverageListingPriceHQ { get; set; }

    public float AverageSold { get; set; }
    public float AverageSoldNQ { get; set; }
    public float AverageSoldHQ { get; set; }

    public PricePoco() { }

    public PricePoco(
        int itemId,
        int worldId,
        long lastUploadTime,
        float currentAveragePrice,
        float currentAveragePriceNQ,
        float currentAveragePriceHQ,
        float averagePrice,
        float averagePriceNQ,
        float averagePriceHQ
    )
    {
        ItemId = itemId;
        WorldId = worldId;
        LastUploadTime = lastUploadTime;
        AverageListingPrice = currentAveragePrice;
        AverageListingPriceNQ = currentAveragePriceNQ;
        AverageListingPriceHQ = currentAveragePriceHQ;
        AverageSold = averagePrice;
        AverageSoldNQ = averagePriceNQ;
        AverageSoldHQ = averagePriceHQ;
    }

    public PricePoco(PriceWebPoco webPoco)
    {
        ItemId = webPoco.ItemId;
        WorldId = webPoco.WorldId;
        LastUploadTime = webPoco.LastUploadTime;
        AverageListingPrice = webPoco.CurrentAveragePrice;
        AverageListingPriceNQ = webPoco.CurrentAveragePriceNQ;
        AverageListingPriceHQ = webPoco.CurrentAveragePriceHQ;
        AverageSold = webPoco.AveragePrice;
        AverageSoldNQ = webPoco.AveragePriceNQ;
        AverageSoldHQ = webPoco.AveragePriceHQ;
    }
}
