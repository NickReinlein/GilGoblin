using System;

namespace GilGoblin.Finance
{
    internal class Price
    {
        private static Random random_gen = new Random();
        public static int Get_Market_Price(int item_id)
        {
            int price = 0;
            //TODO: Use the Universalis API to pull market price data
            //For now we generate a random price to have data available
            price = random_gen.Next(200, 700);
            return price;
        }

    }
}