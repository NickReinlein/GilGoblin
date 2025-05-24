using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;

namespace GilGoblin.Tests.Component;

public class EndpointComponentTests : ComponentTests
{
    [Test]
    public async Task GivenAnApi_WhenHealthEndpointIsCalled_ThenWeReceiveOKResponse()
    {
        const string fullEndpoint = $"{baseUrl}health";

        using var response = await _client.GetAsync(fullEndpoint);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
    
    [TestCaseSource(nameof(_allEndPoints))]
    public async Task GivenACallToGet_WhenTheEndPointIsValid_ThenTheEndpointResponds(
        string endpoint
    )
    {
        var fullEndpoint = $"{baseUrl}{endpoint}";

        var response = await _client.GetAsync(fullEndpoint);

        Assert.That(response.IsSuccessStatusCode, $"{endpoint}");
    }

    private static string[] _allEndPoints =
    [
        "recipe/",
        "recipe/1604",
        "price/21",
        "price/21/1604/true",
        "item/",
        "item/1604",
        "craft/21"
    ];
}