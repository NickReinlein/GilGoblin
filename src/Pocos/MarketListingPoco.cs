using System;

namespace GilGoblin.Pocos;

public class MarketListingPoco
{
    public int ItemId { get; set; }
    public int WorldId { get; set; }
    public DateTime Timestamp { get; set; }
    public bool Hq { get; set; }
    public int Price { get; set; }
    public int Quantity { get; set; }

    public MarketListingPoco() { }

    public MarketListingPoco(int pricePerUnit, int quantity, long lastReviewTime, bool hq)
    {
        Hq = hq;
        Price = pricePerUnit;
        Quantity = quantity;
        Hq = hq;
        try
        {
            Timestamp = new DateTime(lastReviewTime).ToUniversalTime();
        }
        catch (Exception)
        {
            Timestamp = DateTime.Now.AddDays(-7);
        }
    }
}