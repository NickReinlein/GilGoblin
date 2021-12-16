using GilGoblin.WebAPI;
using System;
using System.Collections.Generic;

namespace GilGoblin.Finance
{
    public class MarketDataBase
    {

        internal int CalculateAveragePrice(ICollection<MarketListingWeb> listings)
        {
            int average_Price = 0;
            uint total_Qty = 0;
            ulong total_Gil = 0;
            foreach (MarketListingWeb listing in listings)
            {
                total_Qty += ((uint)listing.qty);
                total_Gil += ((ulong)(listing.price * listing.qty));
            }
            if (total_Qty > 0)
            {
                float average_Price_f = total_Gil / total_Qty;
                average_Price = (int)Math.Round(average_Price_f);
            }
            return average_Price;
        }
    }
}