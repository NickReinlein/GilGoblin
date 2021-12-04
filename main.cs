using System;
using Flurl;
using Flurl.Http;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using GilGoblin.Tests;

class main
{
    static void Main(string[] args)
    {
        test_Calcs.test_Fetch_Market_Price();
    }

}
