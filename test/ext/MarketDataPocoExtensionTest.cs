using GilGoblin.pocos;
using GilGoblin.ext;
using GilGoblin.crafting;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace GilGoblin.Test.ext
{
    [TestFixture]
    public class MarketDataPocoExtensionTest
    {
        private MarketDataPoco _poco = new MarketDataPoco();

        private int ERROR_COST = CraftingCalculator.ERROR_DEFAULT_COST;
        private const int WORLD_ID = 34; // Brynnhildr

        [SetUp]
        public void setUp()
        {
            _poco = _getGoodPoco();
            _poco.AverageSoldNQ = 888;
            _poco.AverageSoldHQ = 1234;
        }

        [Test]
        public void GivenAMarketDataPoco_WhenGettingAverageSalesCostHQ_ReturnHQValue()
        {
            bool isHQ = true;
            var salePrice = isHQ ? _poco.AverageSoldHQ : _poco.AverageSoldNQ;

            var result = _poco.GetAverageSoldPrice(isHQ);

            Assert.That(result, Is.EqualTo(salePrice));
        }

        [Test]
        public void GivenAMarketDataPoco_WhenGettingAverageSalesCostNQ_ReturnNQValue()
        {
            bool isHQ = false;
            var salePrice = isHQ ? _poco.AverageSoldHQ : _poco.AverageSoldNQ;

            var result = _poco.GetAverageSoldPrice(isHQ);

            Assert.That(result, Is.EqualTo(salePrice));
        }

        [Test]
        public void GivenAMarketDataPoco_WhenGettingAverageListingPriceHQ_ReturnHQValue()
        {
            bool isHQ = true;
            var listingPrice = isHQ ? _poco.AverageListingPriceHQ : _poco.AverageListingPriceNQ;

            var result = _poco.GetAverageListingPrice(isHQ);

            Assert.That(result, Is.EqualTo(listingPrice));
        }

        [Test]
        public void GivenAMarketDataPoco_WhenGettingAverageListingPriceNQ_ReturnNQValue()
        {
            bool isHQ = false;
            var listingPrice = isHQ ? _poco.AverageListingPriceHQ : _poco.AverageListingPriceNQ;

            var result = _poco.GetAverageListingPrice(isHQ);

            Assert.That(result, Is.EqualTo(listingPrice));
        }

        private static MarketDataPoco _getGoodPoco()
        {
            return new MarketDataPoco(1, 1, 1, "test", "testRealm", 300, 200, 400, 600, 400, 800);
        }
    }
};
