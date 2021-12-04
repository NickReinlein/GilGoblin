using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flurl.Http;

namespace GilGoblin.WebAPI
{
    /// <summary>
    /// Fetches market information such as market price for the server
    /// </summary>
    internal class Market
    {
        //public static int Get_Market_price(int item_id)
        //{
        //    Task market_task = Fetch_Market_Price(item_id);

        //    int market_price = 1;
        //    return market_price;
        //}
        //public async static Task<Task> Fetch_Market_Price(int item_id)
        //{
        //    Task testTask;
        //    Console.WriteLine("Starting to fetch");
        //    try
        //    {
        //        //HttpResponseMessage req = (HttpResponseMessage) await "https://xivapi.com/Item/1675".GetAsync();
        //        testTask = (Task) await "https://xivapi.com/Item/1675".GetAsync();

        //        //dynamic item = JsonConvert.DeserializeObject(
        //        //    req.Content.ReadAsStringAsync().Result
        //        //);
        //        //Console.Write($"I.Lv {item.LevelItem} {item.Name_en}");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //    finally
        //    {
        //        Console.WriteLine("Returning now");
        //    }
        //    return testTask;
        //}
    }
}
