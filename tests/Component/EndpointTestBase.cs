using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;

namespace GilGoblin.Tests.Component;

public class EndpointTestBase : TestBase
{
    [Test]
    public async Task GivenAnApi_WhenHealthEndpointIsCalled_ThenWeReceiveOKResponse()
    {
        const string fullEndpoint = "health";

        using var response = await _client.GetAsync(fullEndpoint);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
    
    [TestCaseSource(nameof(_allEndPoints))]
    public async Task GivenACallToGet_WhenTheEndPointIsValid_ThenTheEndpointResponds(
        string endpoint
    )
    {
        var fullEndpoint = $"{endpoint}";

        var response = await _client.GetAsync(fullEndpoint);

        Assert.That(response.IsSuccessStatusCode, $"{endpoint}");
    }

    private static string[] _allEndPoints =
    [
        "api/recipe/",
        "api/recipe/1604",
        "api/price/21",
        "api/price/21/1604/true",
        "api/item/",
        "api/item/1604"
    ];
}