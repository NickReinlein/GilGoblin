using System.Text.Json.Serialization;

namespace GilGoblin.Pocos;

public record PricePoco : BasePricePoco
{
    public int ItemID { get; set; }
    public int WorldID { get; set; }

    // The last upload time for this endpoint, in milliseconds since the UNIX epoch.
    public long LastUploadTime { get; set; }
    public float AverageListingPrice { get; set; }
    public float AverageListingPriceNQ { get; set; }
    public float AverageListingPriceHQ { get; set; }

    public float AverageSold { get; set; }
    public float AverageSoldNQ { get; set; }
    public float AverageSoldHQ { get; set; }

    public PricePoco() { }

    [JsonConstructor]
    public PricePoco(
        int itemID,
        int worldID,
        long lastUploadTime,
        float currentAveragePrice,
        float currentAveragePriceNQ,
        float currentAveragePriceHQ,
        float averagePrice,
        float averagePriceNQ,
        float averagePriceHQ
    )
    {
        ItemID = itemID;
        WorldID = worldID;
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
        ItemID = webPoco.ItemID;
        WorldID = webPoco.WorldID;
        LastUploadTime = webPoco.LastUploadTime;
        AverageListingPrice = webPoco.CurrentAveragePrice;
        AverageListingPriceNQ = webPoco.CurrentAveragePriceNQ;
        AverageListingPriceHQ = webPoco.CurrentAveragePriceHQ;
        AverageSold = webPoco.AveragePrice;
        AverageSoldNQ = webPoco.AveragePriceNQ;
        AverageSoldHQ = webPoco.AveragePriceHQ;
    }
}
