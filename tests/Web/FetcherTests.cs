using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace GilGoblin.Tests.Web;

public class FetcherTests
{
    protected HttpClient _client;
    protected MockHttpMessageHandler _handler;

    protected static readonly string ContentType = "application/json";

    [SetUp]
    public virtual void SetUp()
    {
        _handler = new MockHttpMessageHandler();
        _client = _handler.ToHttpClient();
    }
}
