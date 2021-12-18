namespace GilGoblin.Finance
{
    internal abstract class MarketListing
    {
        public bool hq { get; set; }
        public int price { get; set; }
        public int qty { get; set; }

        public MarketListing() { }

    }
}
