using Newtonsoft.Json;

namespace GilGoblin.Pocos
{
    public class MarketDataWebPoco
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
        public decimal averageListingPrice { get; set; }
        // The average NQ listing price, with outliers removed beyond 3 standard deviations of the mean.
        public decimal averagePriceNQ { get; set; }
        // The average HQ listing price, with outliers removed beyond 3 standard deviations of the mean.
        public decimal averagePriceHQ { get; set; }

        // TODO: return later to see which is the best data type for price Poco
        // The average sale price, with outliers removed beyond 3 standard deviations of the mean.
        public float? averageSale { get; set; }
        // The average NQ sale price, with outliers removed beyond 3 standard deviations of the mean.
        public decimal? averageSaleNQ { get; set; }
        // The average HQ sale price, with outliers removed beyond 3 standard deviations of the mean.
        public double? averageSaleHQ { get; set; }
        public MarketDataWebPoco()
        {
        }
        [JsonConstructor]
        public MarketDataWebPoco(int itemID, int worldID, long lastUploadTime, string? name, string? regionName, 
            decimal currentAveragePrice, decimal currentAveragePriceNQ, decimal currentAveragePriceHQ, 
            float? averagePrice, decimal? averagePriceNQ, double? averagePriceHQ)
        {
            this.itemID = itemID;
            this.worldID = worldID;
            this.lastUploadTime = lastUploadTime;
            this.name = name;
            this.regionName = regionName;
            this.averageListingPrice = currentAveragePrice;
            this.averagePriceNQ = currentAveragePriceNQ;
            this.averagePriceHQ = currentAveragePriceHQ;
            this.averageSale = averagePrice;
            this.averageSaleNQ = averagePriceNQ;
            this.averageSaleHQ = averagePriceHQ;
        }

        

        public MarketDataWebPoco(MarketDataWebPoco copyMe)
        {
            this.itemID = copyMe.itemID;
            this.worldID = copyMe.worldID;
            this.lastUploadTime = copyMe.lastUploadTime;
            this.name = copyMe.name;
            this.regionName = copyMe.regionName;
            this.averageListingPrice = copyMe.averageListingPrice;
            this.averagePriceNQ = copyMe.averagePriceNQ;
            this.averagePriceHQ = copyMe.averagePriceHQ;
            this.averageSale = copyMe.averageSale;
            this.averageSaleNQ = copyMe.averageSaleNQ;
            this.averageSaleHQ = copyMe.averageSaleHQ;
        }
    }
}
