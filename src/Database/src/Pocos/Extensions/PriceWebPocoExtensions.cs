using System;
using System.Collections.Generic;
using System.Linq;

namespace GilGoblin.Database.Pocos.Extensions;

public static class PriceWebPocoExtensions
{
    public static List<PriceDataDbPoco> ToDbPocos(this List<PriceWebPoco> webPocos)
    {
        var dbPocos = new List<PriceDataDbPoco>();

        foreach (var webPoco in webPocos)
        {
            var abc = webPoco.Hq?.ToQualityPriceDataList();
        }

        return dbPocos;
    }
}