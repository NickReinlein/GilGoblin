// namespace GilGoblin.Database.Pocos;
//
// public record PricePoco : IIdentifiable
// {
//     public float AverageListingPrice { get; set; }
//     public float AverageListingPriceNQ { get; set; }
//     public float AverageListingPriceHQ { get; set; }
//
//     public float AverageSold { get; set; }
//     public float AverageSoldNQ { get; set; }
//     public float AverageSoldHQ { get; set; }
//     public int WorldId { get; set; }
//     public int ItemId { get; set; }
//
//     // The last upload time for this endpoint, in milliseconds since the UNIX epoch.
//     public long? LastUploadTime { get; set; }
//
//     public int GetId() => ItemId;
//
//     public PricePoco() { }
//
//     public PricePoco(
//         int itemId,
//         int worldId,
//         long lastUploadTime,
//         float currentAveragePrice,
//         float currentAveragePriceNQ,
//         float currentAveragePriceHQ,
//         float averagePrice,
//         float averagePriceNQ,
//         float averagePriceHQ
//     )
//     {
//         ItemId = itemId;
//         WorldId = worldId;
//         LastUploadTime = lastUploadTime;
//         AverageListingPrice = currentAveragePrice;
//         AverageListingPriceNQ = currentAveragePriceNQ;
//         AverageListingPriceHQ = currentAveragePriceHQ;
//         AverageSold = averagePrice;
//         AverageSoldNQ = averagePriceNQ;
//         AverageSoldHQ = averagePriceHQ;
//     }
// }