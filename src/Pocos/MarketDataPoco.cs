using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace GilGoblin.Pocos
{
    public class MarketDataPoco
    {
        // The item ID.
        public int ItemID { get; set; }

        // The world ID, if applicable.
        public int WorldID { get; set; }

        // The last upload time for this endpoint, in milliseconds since the UNIX epoch.
        public long LastUploadTime { get; set; }

        // The DC name, if applicable.
        public string? Name { get; set; }

        // The region name, if applicable.
        public string? RegionName { get; set; }

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

        public MarketDataPoco() { }

        [JsonConstructor]
        public MarketDataPoco(
            int itemID,
            int worldID,
            long lastUploadTime,
            string? name,
            string? regionName,
            float currentAveragePrice,
            float currentAveragePriceNQ,
            float currentAveragePriceHQ,
            float averagePrice,
            float averagePriceNQ,
            float averagePriceHQ
        )
        {
            this.ItemID = itemID;
            this.WorldID = worldID;
            this.LastUploadTime = lastUploadTime;
            this.Name = name;
            this.RegionName = regionName;
            this.AverageListingPrice = currentAveragePrice;
            this.AverageListingPriceNQ = currentAveragePriceNQ;
            this.AverageListingPriceHQ = currentAveragePriceHQ;
            this.AverageSold = averagePrice;
            this.AverageSoldNQ = averagePriceNQ;
            this.AverageSoldHQ = averagePriceHQ;
        }

        public MarketDataPoco(MarketDataPoco copyMe)
        {
            this.ItemID = copyMe.ItemID;
            this.WorldID = copyMe.WorldID;
            this.LastUploadTime = copyMe.LastUploadTime;
            this.Name = copyMe.Name;
            this.RegionName = copyMe.RegionName;
            this.AverageListingPrice = copyMe.AverageListingPrice;
            this.AverageListingPriceNQ = copyMe.AverageListingPriceNQ;
            this.AverageListingPriceHQ = copyMe.AverageListingPriceHQ;
            this.AverageSold = copyMe.AverageSold;
            this.AverageSoldNQ = copyMe.AverageSoldNQ;
            this.AverageSoldHQ = copyMe.AverageSoldHQ;
        }
    }
}
