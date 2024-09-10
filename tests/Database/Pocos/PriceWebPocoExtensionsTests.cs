using System.Collections.Generic;
using System.Linq;
using GilGoblin.Database.Pocos;
using GilGoblin.Database.Pocos.Extensions;
using NUnit.Framework;

namespace GilGoblin.Tests.Database.Pocos;

public class PriceWebPocoExtensionsTests
{
    [Test]
    public void ToDbPocos_ShouldHandleNullData()
    {
        List<PriceWebPoco> webPocos = [new(ItemId: 1, Hq: null, Nq: null)];

        var result = webPocos.ToDbPocos();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void ToDbPocos_ShouldHandleOneEntry()
    {
        var priceDataPoint = new PriceDataPointWebPoco(1, 101, true, null, null, null);
        var qualityDataPoint = new QualityPriceDataPoco(null, priceDataPoint, null, null);
        var webPocos = new List<PriceWebPoco>
        {
            new PriceWebPoco(1 , Hq: priceDataPoint, Nq: null)
        };
    }

    [Test]
    public void ToDbPocos_ShouldHandleMultipleEntries()
    {
        var webPocos = new List<PriceWebPoco>
        {
            new(
                ItemId: 1,
                Hq: new QualityPriceDataPoco(
                    MinListing: new PriceDataPointWebPoco(
                        World: new PriceDataDetailPoco(Price: 100, WorldId: 1, Timestamp: 1234567890),
                        Dc: null,
                        Region: null
                    ),
                    AverageSalePrice: null,
                    RecentPurchase: null,
                    DailySaleVelocity: new DailySaleVelocityWebPoco(1, 2, 3, null)
                ),
                Nq: new QualityPriceDataPoco(
                    MinListing: null,
                    AverageSalePrice: new PriceDataPointWebPoco(
                        World: new PriceDataDetailPoco(Price: 150, WorldId: 2, Timestamp: 1234567890),
                        Dc: null,
                        Region: null
                    ),
                    RecentPurchase: new PriceDataPointWebPoco(
                        World: new PriceDataDetailPoco(Price: 100, WorldId: 1, Timestamp: 1234567890),
                        Dc: null,
                        Region: null
                    ),
                    DailySaleVelocity: new DailySaleVelocityWebPoco(1, 15, 17, 21)
                )
            ),
            new(
                ItemId: 2,
                Hq: null,
                Nq: new QualityPriceDataPoco(
                    MinListing: null,
                    AverageSalePrice: new PriceDataPointWebPoco(
                        World: new PriceDataDetailPoco(Price: 250, Timestamp: 1234567890),
                        Dc: new PriceDataDetailPoco(Price: 307, Timestamp: 1234567890),
                        Region: null
                    ),
                    RecentPurchase: new PriceDataPointWebPoco(
                        World: new PriceDataDetailPoco(Price: 300, Timestamp: 1234567890),
                        Dc: new PriceDataDetailPoco(Price: 400, WorldId: 4, Timestamp: 1234567890),
                        Region: new PriceDataDetailPoco(Price: 500, WorldId: 4, Timestamp: 1234567890)
                    ),
                    DailySaleVelocity: new DailySaleVelocityWebPoco(2, 2, 3, null)
                )
            )
        };

        var result = webPocos.ToDbPocos();

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(6));
            Assert.That(result.All(p => !string.IsNullOrEmpty(p.PriceType)));
            Assert.That(result.All(p => p.Price > 0));
            Assert.That(result.All(p => p.Timestamp > 0));
        });
    }
}