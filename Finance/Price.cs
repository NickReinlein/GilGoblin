using System;
using System.Threading.Tasks;
using GilGoblin.WebAPI;

namespace GilGoblin.Finance
{
    internal class Price
    {
        private static Random random_gen = new Random();
        public static int Get_Market_Price(int item_id, string world_name)
        {
            return Market.Get_Market_Price(item_id, world_name).GetAwaiter().GetResult();
        }

    }
}