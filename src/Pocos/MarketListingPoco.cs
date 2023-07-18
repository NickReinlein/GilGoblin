using System;
using GilGoblin.Services;

namespace GilGoblin.Pocos;

public class MarketListingPoco
{
    public int ItemID { get; set; }
    public int WorldID { get; set; }
    public DateTime Timestamp { get; set; }
    public bool Hq { get; set; }
    public int Price { get; set; }
    public int Quantity { get; set; }

    public MarketListingPoco() { }

    public MarketListingPoco(int pricePerUnit, int quantity, long lastReviewTime, bool hq) : base()
    {
        Hq = hq;
        Price = pricePerUnit;
        Quantity = quantity;
        Hq = hq;
        try
        {
            Timestamp = GeneralFunctions.ConvertLongUnixSecondsToDateTime(lastReviewTime);
        }
        catch (Exception)
        {
            Timestamp = DateTime.Now.AddDays(-7);
        }
    }
}
