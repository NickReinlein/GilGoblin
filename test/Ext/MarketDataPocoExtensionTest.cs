using GilGoblin.Pocos;
using GilGoblin.Ext;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace GilGoblin.Test.Ext
{
    [TestFixture]
    public class MarketDataPocoExtensionTest
    {
        private MarketDataPoco _poco = new();

        [SetUp]
        public void SetUp()
        {
            _poco = GetGoodPoco();
            _poco.AverageSoldNQ = 888;
            _poco.AverageSoldHQ = 1234;
        }

        [Test]
        public void GivenAMarketDataPoco_WhenGettingAverageSalesCostHQ_ReturnHQValue()
        {
            var isHQ = true;
            var salePrice = isHQ ? _poco.AverageSoldHQ : _poco.AverageSoldNQ;

            var result = _poco.GetAverageSoldPrice(isHQ);

            Assert.That(result, Is.EqualTo(salePrice));
        }

        [Test]
        public void GivenAMarketDataPoco_WhenGettingAverageSalesCostNQ_ReturnNQValue()
        {
            var isHQ = false;
            var salePrice = isHQ ? _poco.AverageSoldHQ : _poco.AverageSoldNQ;

            var result = _poco.GetAverageSoldPrice(isHQ);

            Assert.That(result, Is.EqualTo(salePrice));
        }

        [Test]
        public void GivenAMarketDataPoco_WhenGettingAverageListingPriceHQ_ReturnHQValue()
        {
            var isHQ = true;
            var listingPrice = isHQ ? _poco.AverageListingPriceHQ : _poco.AverageListingPriceNQ;

            var result = _poco.GetAverageListingPrice(isHQ);

            Assert.That(result, Is.EqualTo(listingPrice));
        }

        [Test]
        public void GivenAMarketDataPoco_WhenGettingAverageListingPriceNQ_ReturnNQValue()
        {
            var isHQ = false;
            var listingPrice = isHQ ? _poco.AverageListingPriceHQ : _poco.AverageListingPriceNQ;

            var result = _poco.GetAverageListingPrice(isHQ);

            Assert.That(result, Is.EqualTo(listingPrice));
        }

        private static MarketDataPoco GetGoodPoco()
        {
            return new MarketDataPoco(1, 1, 1, "test", "testRealm", 300, 200, 400, 600, 400, 800);
        }
    }
};
