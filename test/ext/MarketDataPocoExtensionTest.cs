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
        }

        [Test]
        public void GivenAMarketDataPoco_WhenGettingAverageSalesCostHQ_ReturnHQValue()
        {
            bool isHQ = true;
            _poco.averageSaleNQ = 888;
            var salePrice = _poco.averageSaleHQ = 1234;

            var result = _poco.GetAverageSoldPrice(isHQ);

            Assert.That(result, Is.EqualTo(salePrice));
        }

        [Test]
        public void GivenAMarketDataPoco_WhenGettingAverageSalesCostNQ_ReturnNQValue()
        {
            bool isHQ = false;
            _poco.averageSaleHQ = 1234;
            var salePrice = _poco.averageSaleNQ = 888;

            var result = _poco.GetAverageSoldPrice(isHQ);

            Assert.That(result, Is.EqualTo(salePrice));
        }

        [Test]
        public void GivenAMarketDataPoco_WhenGettingAverageListingPriceHQ_ReturnHQValue()
        {
            bool isHQ = true;
            _poco.averagePriceNQ = 888;
            var salePrice = _poco.averagePriceHQ = 1234;

            var result = _poco.GetAverageListingPrice(isHQ);

            Assert.That(result, Is.EqualTo(salePrice));
        }

        [Test]
        public void GivenAMarketDataPoco_WhenGettingAverageListingPriceNQ_ReturnNQValue()
        {
            bool isHQ = false;
            _poco.averageSaleHQ = 1234;
            _poco.averageSaleNQ = 888;
            var salePrice = isHQ ? _poco.averageSaleHQ : _poco.averageSaleNQ;

            var result = _poco.GetAverageListingPrice(isHQ);

            Assert.That(result, Is.EqualTo(salePrice));
        }

        private static MarketDataPoco _getGoodPoco()
        {
            return new MarketDataPoco(1, 1, 1, "test", "testRealm", 300, 200, 400, 600, 400, 800);
        }
    }
};
