namespace GilGoblin.Pocos;

public record PriceWebPoco : BasePricePoco
{
    public int ItemID { get; set; }
    public int WorldID { get; set; }

    // The last upload time for this endpoint, in milliseconds since the UNIX epoch.
    public long LastUploadTime { get; set; }

    // The average listing price, with outliers removed beyond 3 standard deviations of the mean.
    public float CurrentAveragePrice { get; set; }
    public float CurrentAveragePriceNQ { get; set; }
    public float CurrentAveragePriceHQ { get; set; }

    // The average sale price, with outliers removed beyond 3 standard deviations of the mean.
    public float AveragePrice { get; set; }
    public float AveragePriceNQ { get; set; }
    public float AveragePriceHQ { get; set; }

    public PriceWebPoco(
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
        CurrentAveragePrice = currentAveragePrice;
        CurrentAveragePriceNQ = currentAveragePriceNQ;
        CurrentAveragePriceHQ = currentAveragePriceHQ;
        AveragePrice = averagePrice;
        AveragePriceNQ = averagePriceNQ;
        AveragePriceHQ = averagePriceHQ;
    }
}