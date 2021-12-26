using GilGoblin.Database;
using GilGoblin.WebAPI;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GilGoblin.Finance
{
    /// <summary>
    /// Manages market information such as market price 
    /// </summary>
    internal class MarketData
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        public int worldID { get; set; }

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [ForeignKey("itemId")]
        [InverseProperty("ItemDB")]
        public int itemID { get; set; }
        public DateTime lastUpdated { get; set; }

        public static int _staleness_hours_for_refresh = 2; //todo: increase for production        

        /// <summary>
        /// Given the collection of listings, calculate their average price
        /// </summary>
        /// <param name="listings"></param>
        /// <returns></returns>
        public int CalculateAveragePrice(ICollection<MarketListingWeb> listings)
        {
            if (listings.Count == 0) { return 0; }
            int average_Price = 0;
            uint total_Qty = 0;
            ulong total_Gil = 0;
            try
            {
                foreach (MarketListingWeb listing in listings)
                {
                    if ((listing.qty > 1 && listing.price > 200000) ||
                        (listing.qty == 1 && listing.price > 400000))
                    {
                        continue; //Exclude crazy prices
                    }

                    total_Qty += ((uint)listing.qty);
                    total_Gil += ((ulong)(listing.price * listing.qty));
                }
                if (total_Qty > 0)
                {
                    float average_Price_f = total_Gil / total_Qty;
                    average_Price = (int)Math.Round(average_Price_f);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error calculating average price for item {itemID} world {worldID}. Message: {message}", this.itemID, this.worldID, ex.Message);
                average_Price = 0;
            }
            return average_Price;
        }
    }

}


