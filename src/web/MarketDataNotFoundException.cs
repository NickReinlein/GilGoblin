using System;
namespace GilGoblin.web 
{
    public class MarketDataNotFoundException : Exception
    {
        public MarketDataNotFoundException()
        {
        }

        public MarketDataNotFoundException(string message)
            : base(message)
        {
        }

        public MarketDataNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}