using System;
using Flurl;
using Flurl.Http;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;

class main
{
    static void Main(string[] args)
    {
        Fetch();
    }

    static async void Fetch() 
    {
        Console.WriteLine("Starting to fetch");
        try
        {
            //HttpResponseMessage req = (HttpResponseMessage) await "https://xivapi.com/Item/1675".GetAsync();
            Task testTask = (Task) await "https://xivapi.com/Item/1675".GetAsync();

            //dynamic item = JsonConvert.DeserializeObject(
            //    req.Content.ReadAsStringAsync().Result
            //);
            //Console.Write($"I.Lv {item.LevelItem} {item.Name_en}");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            Console.WriteLine("Returning now");
        }
    }
}
