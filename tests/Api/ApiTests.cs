// using GilGoblin.Api;
// using GilGoblin.Database;
// using Microsoft.AspNetCore.Hosting;
// using Microsoft.AspNetCore.Mvc.Testing;
// using Microsoft.AspNetCore.TestHost;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.DependencyInjection;
// using NUnit.Framework;

// namespace GilGoblin.Tests.Api;

// public class EndpointTests
// {
//     private WebApplicationFactory<Startup> _factory;
//     private HttpClient _client;

//     public HttpClient Get_client()
//     {
//         return _client;
//     }

//     [SetUp]
//     public void SetUp()
//     {
//         _factory = new WebApplicationFactory<Startup>();
//         // _factory = new WebApplicationFactory<Startup>().WithWebHostBuilder(builder =>
//         // {
//         //     builder.ConfigureTestServices(services =>
//         //     {
//         //         var options = new DbContextOptionsBuilder<GilGoblinDbContext>()
//         //             .UseSqlite("Data Source=../../resources/GilGoblin.db;")
//         //             .Options;

//         //         services.AddSingleton(_ => new GilGoblinDbContext(options));
//         //     });
//         // });

//         _client = _factory.CreateClient();
//     }

//     [TearDown]
//     public void OneTimeTearDown()
//     {
//         _client.CancelPendingRequests();
//         _client.Dispose();
//         _factory.Dispose();
//     }

//     // [TestCase("/recipe/")]
//     // [TestCase("/recipe/100")]
//     // [TestCase("/price/34/")]
//     // [TestCase("/price/34/100")]
//     // [TestCase("/craft/34/")]
//     // [TestCase("/craft/34/100")]
//     // [TestCase("/item/")]
//     [TestCase("/item/100")]
//     public async Task WhenWeResolveEndpoints_ThenEachEndpointResponds(string endpoint)
//     {
//         var fullEndpoint = $"http://localhost:55448{endpoint}";

//         using var response = await _client.GetAsync(fullEndpoint);

//         response.EnsureSuccessStatusCode();
//     }
// }
