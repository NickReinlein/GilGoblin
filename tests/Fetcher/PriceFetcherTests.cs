using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;
using GilGoblin.Fetcher;
using GilGoblin.Fetcher.Pocos;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace GilGoblin.Tests.Fetcher;

public class PriceFetcherTests : FetcherTests
{
    private PriceFetcher _fetcher;
    private ILogger<PriceFetcher> _logger;
    
    private const int worldId = 34;
    private const int regionId = 56;
    private const int itemId1 = 4211;
    private const int itemId2 = 4222;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _logger = Substitute.For<ILogger<PriceFetcher>>();
        _fetcher = new PriceFetcher(_logger, _client);
    }

    #region Fetcher calls

    [Test]
    public async Task GivenWeCallFetchMultiplePricesAsync_WhenTheResponseIsValid_ThenWeDeserializeSuccessfully()
    {
        var idList = SetupResponse();

        var result = await _fetcher.FetchByIdsAsync(CancellationToken.None, idList, worldId);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Any(r => r.ItemId == itemId1));
            Assert.That(result.Any(r => r.ItemId == itemId2));
            Assert.That(result.All(r => r.WorldUploadTimes?.Count > 0), Is.True);
            // Assert.That(result.All(r => r.Hq?.AverageSalePrice?.Price > 0), Is.True);
            // Assert.That(result.All(r => r.Nq?.AverageSalePrice?.Price > 0), Is.True);
        });
    }


    [Test]
    public async Task
        GivenWeCallFetchMultiplePricesAsync_WhenTheResponseIsStatusCodeIsUnsuccessful_ThenWeReturnAnEmptyList()
    {
        var returnedList = GetPocoList().ToList();
        var idList = returnedList.Select(i => i.ItemId);
        _handler
            .When(FetchPricesAsyncUrl)
            .Respond(HttpStatusCode.NotFound, ContentType, JsonSerializer.Serialize(returnedList));

        var result = await _fetcher.FetchByIdsAsync(CancellationToken.None, idList, worldId);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GivenWeCallFetchMultiplePricesAsync_WhenNoIDsAreProvided_ThenWeReturnAnEmptyList()
    {
        var result = await _fetcher.FetchByIdsAsync(CancellationToken.None, Array.Empty<int>(), worldId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GivenWeCallFetchMultiplePricesAsync_WhenTheResponseIsInvalid_ThenWeReturnAnEmptyList()
    {
        var idList = GetPocoList().Select(i => i.ItemId).ToList();
        _handler
            .When(FetchPricesAsyncUrl)
            .Respond(
                HttpStatusCode.OK,
                ContentType,
                JsonSerializer.Serialize(GetPocoList())
            );

        var result = await _fetcher.FetchByIdsAsync(CancellationToken.None, idList, worldId);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GivenWeCallFetchMultiplePricesAsync_WhenTheResponseIsNull_ThenWeReturnAnEmptyList()
    {
        var idList = GetPocoList().Select(i => i.ItemId).ToList();
        _handler
            .When(FetchPricesAsyncUrl)
            .Respond(HttpStatusCode.OK, ContentType, "{ alksdfjs }");

        var result = await _fetcher.FetchByIdsAsync(CancellationToken.None, idList, worldId);

        Assert.That(result, Is.Empty);
    }

    #endregion

    #region Deserialization

    [Test]
    public void GivenWeDeserializeAResponse_WhenMultipleValidEntities_ThenWeDeserializeSuccessfully()
    {
        var ids = new[] { itemId1, itemId2 };
        var result = JsonSerializer.Deserialize<PriceWebResponse>(
            GetItemsJsonResponse,
            GetSerializerOptions()
        );

        var prices = result?.GetContentAsList();

        Assert.That(prices, Is.Not.Null.And.Not.Empty);
        foreach (var itemId in ids)
        {
            var price = prices.FirstOrDefault(p => p.ItemId == itemId);
            Assert.That(price, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(ids, Does.Contain(price.ItemId));
                Assert.That(price.WorldUploadTimes?.Count, Is.GreaterThan(0));
                // Assert.That(price.Hq?.AverageSalePrice?.Price, Is.GreaterThan(0));
                // Assert.That(price.Nq?.AverageSalePrice?.Price, Is.GreaterThan(0));
                // Assert.That(price.WorldUploadTimes?.Count, Is.GreaterThan(0));
                // Assert.That(price.WorldUploadTimes?.All(w => w.WorldId > 0), Is.True);
                // Assert.That(price.WorldUploadTimes?.All(w => w.Timestamp > 0), Is.True);
            });
        }
    }

    [Test]
    public void GivenWeDeserializeAResponse_WhenAValidMarketableEntity_ThenWeDeserialize()
    {
        var result = JsonSerializer.Deserialize<List<int>>(
            GetItemsJsonResponseMarketable,
            GetSerializerOptions()
        );

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result?.All(i => i > 0), Is.True);
    }

    #endregion

    private IEnumerable<int> SetupResponse(bool success = true, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var pocoList = GetPocoList();
        var idList = pocoList.Select(i => i.ItemId).ToList();
        var webResponse = new PriceWebResponse(pocoList, []);
        var responseContent
            = success
                ? JsonSerializer.Serialize(webResponse)
                : JsonSerializer.Serialize(idList);

        _handler
            .When(FetchPricesAsyncUrl)
            .Respond(statusCode, ContentType, responseContent);
        return idList;
    }

    protected static List<PriceWebPoco> GetPocoList()
    {
        var priceGeoDataPointsPoco = new PriceDataPointWebPoco(
            new PriceDataDetailPoco(900),
            new PriceDataDetailPoco(800, worldId),
            new PriceDataDetailPoco(700, regionId)
        );
        var anyQ = new QualityPriceDataPoco(
            priceGeoDataPointsPoco,
            priceGeoDataPointsPoco,
            priceGeoDataPointsPoco,
            new DailySaleVelocityWebPoco(50, 200f, 300f, 400f)
        );
        var worldUploadTimestampPocos = new List<WorldUploadTimeWebPoco>
        {
            new(worldId, 67554), new(worldId + 1, 67555)
        };

        var poco1 = new PriceWebPoco(
            itemId1,
            anyQ,
            anyQ,
            worldUploadTimestampPocos
        );
        var poco2 = new PriceWebPoco(
            itemId2,
            anyQ,
            anyQ,
            worldUploadTimestampPocos
        );

        return [poco1, poco2];
    }

    protected static JsonSerializerOptions GetSerializerOptions() =>
        new() { PropertyNameCaseInsensitive = true, IncludeFields = true };

    private static string FetchPricesAsyncUrl =>
        $"https://universalis.app/api/v2/aggregated/{worldId}/{itemId1},{itemId2}";

    private static string GetItemsJsonResponseMarketable => $"[{itemId1},{itemId2}]";

    private static string GetItemsJsonResponse =>
        """
        {
        	"results": [
        		{
        			"itemId": 4211,
        			"nq": {
        				"minListing": {},
        				"recentPurchase": {
        					"dc": {
        						"price": 4000,
        						"timestamp": 1723285302000,
        						"worldId": 21
        					},
        					"region": {
        						"price": 4000,
        						"timestamp": 1723285302000,
        						"worldId": 21
        					}
        				},
        				"averageSalePrice": {},
        				"dailySaleVelocity": {}
        			},
        			"hq": {
        				"minListing": {
        					"dc": {
        						"price": 30000,
        						"worldId": 86
        					},
        					"region": {
        						"price": 30000,
        						"worldId": 86
        					}
        				},
        				"recentPurchase": {},
        				"averageSalePrice": {},
        				"dailySaleVelocity": {}
        			},
        			"worldUploadTimes": [
        				{
        					"worldId": 86,
        					"timestamp": 1722677228827
        				}
        			]
        		},
        		{
        			"itemId": 4222,
        			"nq": {
        				"minListing": {
        					"dc": {
        						"price": 2500,
        						"worldId": 22
        					},
        					"region": {
        						"price": 2500,
        						"worldId": 22
        					}
        				},
        				"recentPurchase": {
        					"world": {
        						"price": 3500,
        						"timestamp": 1721394336000
        					},
        					"dc": {
        						"price": 9000,
        						"timestamp": 1724028677000,
        						"worldId": 21
        					},
        					"region": {
        						"price": 9000,
        						"timestamp": 1724028677000,
        						"worldId": 21
        					}
        				},
        				"averageSalePrice": {},
        				"dailySaleVelocity": {}
        			},
        			"hq": {
        				"minListing": {
        					"world": {
        						"price": 5000
        					},
        					"dc": {
        						"price": 5000,
        						"worldId": 87
        					},
        					"region": {
        						"price": 5000,
        						"worldId": 87
        					}
        				},
        				"recentPurchase": {
        					"world": {
        						"price": 5000,
        						"timestamp": 1722299235000
        					},
        					"dc": {
        						"price": 20000,
        						"timestamp": 1723040451000,
        						"worldId": 21
        					},
        					"region": {
        						"price": 20000,
        						"timestamp": 1723040451000,
        						"worldId": 21
        					}
        				},
        				"averageSalePrice": {},
        				"dailySaleVelocity": {}
        			},
        			"worldUploadTimes": [
        				{
        					"worldId": 87,
        					"timestamp": 1723122140142
        				},
        				{
        					"worldId": 22,
        					"timestamp": 1723810729785
        				}
        			]
        		}
        	],
        	"failedItems": []
        }
        """;
}