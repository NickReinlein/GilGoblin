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
        public string? Name { get; set; }
        // The region name, if applicable.
        public string? regionName { get; set; }

        // The average listing price, with outliers removed beyond 3 standard deviations of the mean.
        public decimal currentAveragePrice { get; set; }
        // The average NQ listing price, with outliers removed beyond 3 standard deviations of the mean.
        public decimal currentAveragePriceNQ { get; set; }
        // The average HQ listing price, with outliers removed beyond 3 standard deviations of the mean.
        public decimal currentAveragePriceHQ { get; set; }

        // TODO: return later to see which is the best data type for price Poco
        // The average sale price, with outliers removed beyond 3 standard deviations of the mean.
        public float? averagePrice { get; set; }
        // The average NQ sale price, with outliers removed beyond 3 standard deviations of the mean.
        public decimal? averagePriceNQ { get; set; }
        // The average HQ sale price, with outliers removed beyond 3 standard deviations of the mean.
        public double? averagePriceHQ { get; set; }
        public MarketDataWebPoco()
        {
        }

        
    }
}
