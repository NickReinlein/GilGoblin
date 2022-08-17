using Newtonsoft.Json;

namespace GilGoblin.pocos
{
    public class MarketDataPoco
    {
        // The item ID.
        public int itemID { get; set; }

        // The world ID, if applicable.
        public int worldID { get; set; }

        // The last upload time for this endpoint, in milliseconds since the UNIX epoch.
        public long lastUploadTime { get; set; }

        // The DC name, if applicable.
        public string? name { get; set; }

        // The region name, if applicable.
        public string? regionName { get; set; }

        // The average listing price, with outliers removed beyond 3 standard deviations of the mean.
        public float averageListingPrice { get; set; }

        // The average NQ listing price, with outliers removed beyond 3 standard deviations of the mean.
        public float averageListingPriceNQ { get; set; }

        // The average HQ listing price, with outliers removed beyond 3 standard deviations of the mean.
        public float averageListingPriceHQ { get; set; }

        // The average sale price, with outliers removed beyond 3 standard deviations of the mean.
        public float averageSold { get; set; }

        // The average NQ sale price, with outliers removed beyond 3 standard deviations of the mean.
        public float averageSoldNQ { get; set; }

        // The average HQ sale price, with outliers removed beyond 3 standard deviations of the mean.
        public float averageSoldHQ { get; set; }

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
            this.itemID = itemID;
            this.worldID = worldID;
            this.lastUploadTime = lastUploadTime;
            this.name = name;
            this.regionName = regionName;
            this.averageListingPrice = currentAveragePrice;
            this.averageListingPriceNQ = currentAveragePriceNQ;
            this.averageListingPriceHQ = currentAveragePriceHQ;
            this.averageSold = averagePrice;
            this.averageSoldNQ = averagePriceNQ;
            this.averageSoldHQ = averagePriceHQ;
        }

        public MarketDataPoco(MarketDataPoco copyMe)
        {
            this.itemID = copyMe.itemID;
            this.worldID = copyMe.worldID;
            this.lastUploadTime = copyMe.lastUploadTime;
            this.name = copyMe.name;
            this.regionName = copyMe.regionName;
            this.averageListingPrice = copyMe.averageListingPrice;
            this.averageListingPriceNQ = copyMe.averageListingPriceNQ;
            this.averageListingPriceHQ = copyMe.averageListingPriceHQ;
            this.averageSold = copyMe.averageSold;
            this.averageSoldNQ = copyMe.averageSoldNQ;
            this.averageSoldHQ = copyMe.averageSoldHQ;
        }
    }
}
