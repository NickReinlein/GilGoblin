using GilGoblin.Database;
using GilGoblin.Tests;

class main
{
    static void Main(string[] args)
    {
        DatabaseAccess.Startup();
        //testCalcs.test_Fetch_Market_Price();
        testCalcs.test_Fetch_Market_Prices();

    }

}
