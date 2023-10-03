using System.Text.Json.Serialization;

namespace GilGoblin.Pocos;

public class PriceWebPoco : BasePricePoco
{
    // The average listing price, with outliers removed beyond 3 standard deviations of the mean.
    public float CurrentAveragePrice { get; set; }
    public float CurrentAveragePriceNQ { get; set; }
    public float CurrentAveragePriceHQ { get; set; }

    // The average sale price, with outliers removed beyond 3 standard deviations of the mean.
    public float AveragePrice { get; set; }
    public float AveragePriceNQ { get; set; }
    public float AveragePriceHQ { get; set; }

    public PriceWebPoco() { }

    [JsonConstructor]
    public PriceWebPoco(
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
        CurrentAveragePrice = currentAveragePrice;
        CurrentAveragePriceNQ = currentAveragePriceNQ;
        CurrentAveragePriceHQ = currentAveragePriceHQ;
        AveragePrice = averagePrice;
        AveragePriceNQ = averagePriceNQ;
        AveragePriceHQ = averagePriceHQ;
    }
}
