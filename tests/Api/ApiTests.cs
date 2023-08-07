// using System.Net;
// using GilGoblin.Api;
// using Microsoft.AspNetCore.Builder;
// using Microsoft.AspNetCore.Hosting;
// using Microsoft.AspNetCore.TestHost;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.Hosting;
// using NSubstitute;
// using NUnit.Framework;

// namespace GilGoblin.Tests.Api;

// public class EndpointTests
// {
//     private TestServer _server;
//     private HttpClient _client;

//     [OneTimeSetUp]
//     public void OneTimeSetUp()
//     {
//         _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
//         _server.Host.Start();

//         _client = _server.CreateClient();
//     }

//     [OneTimeTearDown]
//     public void OneTimeTearDown()
//     {
//         _client.CancelPendingRequests();
//         _client.Dispose();
//         _server.Dispose();
//     }

//     // [TestCase("/item/")]
//     [TestCase("/item/100")]
//     // [TestCase("/recipe/")]
//     // [TestCase("/recipe/100")]
//     // [TestCase("/price/34/")]
//     // [TestCase("/price/34/100")]
//     // [TestCase("/craft/34/")]
//     // [TestCase("/craft/34/100")]
//     public async Task WhenWeResolveEndpoints_ThenEachEndpointResponds(string endpoint)
//     {
//         var fullEndpoint = $"http://localhost:55448{endpoint}";
//         using var response = await _client.GetAsync(fullEndpoint);

//         response.EnsureSuccessStatusCode();
//     }
// }
