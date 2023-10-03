// using System.Net;
// using System.Text.Json;
// using GilGoblin.Pocos;
// using GilGoblin.Repository;
// using GilGoblin.Web;
// using Microsoft.Extensions.Logging;
// using NSubstitute;
// using NUnit.Framework;
// using RichardSzalay.MockHttp;
//
// namespace GilGoblin.Tests.Web;
//
// public class ItemInfoFetcherTests : FetcherTests
// {
//     private ItemInfoSingleFetcher _fetcher;
//     private IItemRepository _repo;
//     private IMarketableItemIdsFetcher _marketableIdsFetcher;
//     private ILogger<ItemInfoSingleFetcher> _logger;
//
//     [SetUp]
//     public override void SetUp()
//     {
//         base.SetUp();
//         var pocos = GetMultipleDbPocos().ToList();
//         var idList = pocos.Select(i => i.Id).ToList();
//         _logger = Substitute.For<ILogger<ItemInfoSingleFetcher>>();
//         _repo = Substitute.For<IItemRepository>();
//         _repo.GetAll().Returns(pocos);
//         _marketableIdsFetcher = Substitute.For<IMarketableItemIdsFetcher>();
//         _marketableIdsFetcher.GetMarketableItemIdsAsync().Returns(idList);
//         _fetcher = new ItemInfoSingleFetcher(_repo, _marketableIdsFetcher, _logger, _client);
//     }
//
//     #region Basic
//
//     [Test]
//     public void GivenWeCreateAItemInfoFetcher_WhenTheClientIsNotProvided_ThenWeCanUseADefaultClientInstead()
//     {
//         var defaultItemInfos = _fetcher.ItemInfosPerPage;
//
//         _fetcher = new ItemInfoSingleFetcher(_repo, _marketableIdsFetcher, _logger);
//
//         Assert.That(_fetcher.ItemInfosPerPage, Is.GreaterThanOrEqualTo(defaultItemInfos));
//     }
//
//     #endregion
//
//     #region Web calls
//
//     [Test]
//     public async Task GivenWeCallFetchMultipleItemInfosAsync_WhenTheResponseIsValid_ThenWeDeserializeSuccessfully()
//     {
//         var idList = SetupResponse();
//
//         var result = await _fetcher.FetchByIdsAsync(idList);
//
//         Assert.Multiple(() =>
//         {
//             Assert.That(result, Is.Not.Null);
//             Assert.That(result.Any(r => r.Id == ItemId1));
//             Assert.That(result.Any(r => r.Id == ItemId2));
//         });
//     }
//
//
//     [Test]
//     public async Task
//         GivenWeCallFetchMultipleItemInfosAsync_WhenTheResponseIsStatusCodeIsUnsuccessful_ThenWeReturnAnEmptyList()
//     {
//         var returnedList = GetMultipleNewPocos().ToList();
//         var idList = returnedList.Select(i => i.Id);
//         _handler
//             .When(GetUrl)
//             .Respond(HttpStatusCode.NotFound, ContentType, JsonSerializer.Serialize(returnedList));
//
//         var result = await _fetcher.FetchByIdsAsync(idList);
//
//         Assert.That(result, Is.Empty);
//     }
//
//     [Test]
//     public async Task GivenWeCallFetchMultipleItemInfosAsync_WhenNoIdsAreProvided_ThenWeReturnAnEmptyList()
//     {
//         var result = await _fetcher.FetchByIdsAsync(Array.Empty<int>());
//
//         Assert.That(result, Is.Not.Null);
//         Assert.That(result, Is.Empty);
//     }
//
//     [Test]
//     public async Task GivenWeCallFetchMultipleItemInfosAsync_WhenTheResponseIsInvalid_ThenWeReturnAnEmptyList()
//     {
//         var idList = GetMultipleNewPocos().Select(i => i.Id).ToList();
//         _handler
//             .When(GetUrl)
//             .Respond(
//                 HttpStatusCode.OK,
//                 ContentType,
//                 JsonSerializer.Serialize(GetMultipleNewPocos())
//             );
//
//         var result = await _fetcher.FetchByIdsAsync(idList);
//
//         Assert.That(result, Is.Empty);
//     }
//
//     [Test]
//     public async Task GivenWeCallFetchMultipleItemInfosAsync_WhenTheResponseIsNull_ThenWeReturnAnEmptyList()
//     {
//         var idList = GetMultipleNewPocos().Select(i => i.Id).ToList();
//         _handler
//             .When(GetUrl)
//             .Respond(HttpStatusCode.OK, ContentType, "{ alksdfjs }");
//
//         var result = await _fetcher.FetchByIdsAsync(idList);
//
//         Assert.That(result, Is.Empty);
//     }
//
//
//     [Test]
//     public async Task GivenWeCallGetAllIdsAsBatchJobsAsync_WhenTheIDListReturnedIsEmpty_ThenWeReturnAnEmptyResult()
//     {
//         _handler.When(Arg.Any<string>()).Respond(HttpStatusCode.OK, ContentType, "{}");
//
//         var result = await _fetcher.GetIdsAsBatchJobsAsync();
//
//         Assert.That(result, Is.Empty);
//     }
//
//     [Test]
//     public async Task GivenWeCallGetAllIdsAsBatchJobsAsync_WhenTheIDListReturnedIsValid_ThenWeReturnBatchesOfIds()
//     {
//         _handler
//             .When(Arg.Any<string>())
//             .Respond(
//                 HttpStatusCode.OK,
//                 ContentType,
//                 JsonSerializer.Serialize(new List<int> { 1, 2, 3 })
//             );
//
//         var result = await _fetcher.GetIdsAsBatchJobsAsync();
//
//         Assert.Multiple(() =>
//         {
//             Assert.That(result.Count, Is.EqualTo(1));
//             Assert.That(result.First().Count, Is.EqualTo(3));
//         });
//     }
//
//     [TestCase(1, 1)]
//     [TestCase(2, 1)]
//     [TestCase(3, 1)]
//     [TestCase(4, 2)]
//     [TestCase(10, 4)]
//     public async Task GivenWeCallGetAllIdsAsBatchJobsAsync_WhenTheIDListReturnedIsValid_ThenWeReturnIdsBatchedCorrectly(
//         int entryCount,
//         int expectedPages
//     )
//     {
//         _fetcher.ItemInfosPerPage = 3;
//         var idList = Enumerable.Range(1, entryCount);
//         _handler
//             .When(Arg.Any<string>())
//             .Respond(HttpStatusCode.OK, ContentType, JsonSerializer.Serialize(idList));
//
//         var result = await _fetcher.GetIdsAsBatchJobsAsync();
//
//         Assert.That(result, Has.Count.EqualTo(expectedPages));
//     }
//
//     #endregion
//
//     #region Deserialization
//
//     [Test]
//     public void GivenWeDeserializeAResponse_WhenMultipleValidEntities_ThenWeDeserializeSuccessfully()
//     {
//         var result = JsonSerializer.Deserialize<ItemInfoWebResponse>(
//             GetItemsJsonResponse,
//             GetSerializerOptions()
//         );
//
//         var items = result?.GetContentAsList();
//
//         Assert.Multiple(() =>
//         {
//             Assert.That(items, Is.Not.Null.And.Not.Empty);
//             Assert.That(items!.Exists(item => item.Id == ItemId1));
//             Assert.That(items.Exists(item => item.Id == ItemId2));
//         });
//     }
//
//     [Test]
//     public void GivenWeDeserializeAResponse_WhenAValidMarketableEntity_ThenWeDeserialize()
//     {
//         var result = JsonSerializer.Deserialize<List<int>>(
//             GetItemsJsonResponse(),
//             GetSerializerOptions()
//         );
//
//         Assert.That(result, Has.Count.GreaterThan(50));
//         Assert.That(result?.All(i => i > 0), Is.True);
//     }
//
//     #endregion
//
//     private IEnumerable<int> SetupResponse(bool success = true, HttpStatusCode statusCode = HttpStatusCode.OK)
//     {
//         var pocoList = GetMultipleNewPocos().ToList();
//         var idList = pocoList.Select(i => i.Id).ToList();
//         var dict = pocoList.ToDictionary(l => l.Id);
//         var webResponse = new ItemInfoWebResponse(dict);
//         var responseContent = success ? JsonSerializer.Serialize(webResponse) : JsonSerializer.Serialize(idList);
//
//         _handler
//             .When(GetUrl)
//             .Respond(statusCode, ContentType, responseContent);
//         return idList;
//     }
//
//     private static JsonSerializerOptions GetSerializerOptions() =>
//         new() { PropertyNameCaseInsensitive = true, IncludeFields = true };
//
//     private static int ItemId1 => 10972;
//     private static int ItemId2 => 10973;
//
//     private string GetUrl => $"https://xivapi.com/item/{ItemId1}";
//
//     private static string GetItemsJsonResponse =>
//         """{"CanBeHq":1,"Description":"","ID":10972,"IconID":55724,"LevelItem":133,"Name":"Hardsilver Bangle of Fending","PriceLow":119,"PriceMid":20642,"StackSize":1}""";
// }

