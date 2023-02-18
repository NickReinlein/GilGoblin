using GilGoblin.Services;
using Serilog;

namespace GilGoblin.Pocos;

internal class MarketListingPoco
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
        if (Timestamp.Equals(0))
        {
            Log.Error("Empty timestamp for web listing.");
        }
        else
        {
            try
            {
                Timestamp = GeneralFunctions.ConvertLongUnixSecondsToDateTime(lastReviewTime);
            }
            catch (Exception ex)
            {
                Log.Error("Error converting web listing's timestamp of:  {this.timestamp}");
                Log.Error(ex.Message);
            }
        }
    }
}
