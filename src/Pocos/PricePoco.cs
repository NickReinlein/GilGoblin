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
        float averageListingPrice,
        float averageListingPriceNQ,
        float averageListingPriceHQ,
        float averagePrice,
        float averagePriceNQ,
        float averagePriceHQ
    )
    {
        this.ItemID = itemID;
        this.WorldID = worldID;
        this.LastUploadTime = lastUploadTime;
        this.AverageListingPrice = averageListingPrice;
        this.AverageListingPriceNQ = averageListingPriceNQ;
        this.AverageListingPriceHQ = averageListingPriceHQ;
        this.AverageSold = averagePrice;
        this.AverageSoldNQ = averagePriceNQ;
        this.AverageSoldHQ = averagePriceHQ;
    }

    public PricePoco(PricePoco copyMe)
    {
        this.ItemID = copyMe.ItemID;
        this.WorldID = copyMe.WorldID;
        this.LastUploadTime = copyMe.LastUploadTime;
        this.AverageListingPrice = copyMe.AverageListingPrice;
        this.AverageListingPriceNQ = copyMe.AverageListingPriceNQ;
        this.AverageListingPriceHQ = copyMe.AverageListingPriceHQ;
        this.AverageSold = copyMe.AverageSold;
        this.AverageSoldNQ = copyMe.AverageSoldNQ;
        this.AverageSoldHQ = copyMe.AverageSoldHQ;
    }
}
