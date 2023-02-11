using System.Text.Json.Serialization;

namespace GilGoblin.Pocos;

public class PricePoco
{
    // The item ID.
    public int ItemID { get; set; }

    // The world ID, if applicable.
    public int WorldID { get; set; }

    // The last upload time for this endpoint, in milliseconds since the UNIX epoch.
    public long LastUploadTime { get; set; }

    // The average listing price, with outliers removed beyond 3 standard deviations of the mean.
    public float AverageListingPrice { get; set; }

    // The average NQ listing price, with outliers removed beyond 3 standard deviations of the mean.
    public float AverageListingPriceNQ { get; set; }

    // The average HQ listing price, with outliers removed beyond 3 standard deviations of the mean.
    public float AverageListingPriceHQ { get; set; }

    // The average sale price, with outliers removed beyond 3 standard deviations of the mean.
    public float AverageSold { get; set; }

    // The average NQ sale price, with outliers removed beyond 3 standard deviations of the mean.
    public float AverageSoldNQ { get; set; }

    // The average HQ sale price, with outliers removed beyond 3 standard deviations of the mean.
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

    public PricePoco(PricePoco copyMe)
    {
        ItemID = copyMe.ItemID;
        WorldID = copyMe.WorldID;
        LastUploadTime = copyMe.LastUploadTime;
        AverageListingPrice = copyMe.AverageListingPrice;
        AverageListingPriceNQ = copyMe.AverageListingPriceNQ;
        AverageListingPriceHQ = copyMe.AverageListingPriceHQ;
        AverageSold = copyMe.AverageSold;
        AverageSoldNQ = copyMe.AverageSoldNQ;
        AverageSoldHQ = copyMe.AverageSoldHQ;
    }
}
