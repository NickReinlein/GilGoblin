using GilGoblin.Web;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Web;

public class ItemInfoFetcherTests
{
    private readonly ItemInfoFetcher fetcher;

    public ItemInfoFetcherTests()
    {
        fetcher = new ItemInfoFetcher();
    }

    [SetUp]
    public void SetUp() { }

    [TearDown]
    public void TearDown() { }

    [Test]
    public async Task WhenWeGetAnItem_WeCallUsingCorrectParameters()
    {
        var result = await fetcher.GetAsync(_getPath);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.ID, Is.EqualTo(int.Parse(_getPath)));
    }

    [Test]
    public async Task WhenWeGetAllItems_WeCallUsingCorrectParameters()
    {
        var result = await fetcher.GetAllAsync(_getAllPath);

        Assert.That(result, Is.Not.Null);
    }

    private static readonly string _fullPathGet = string.Concat(
        ItemInfoFetcher.ItemInfoBaseUrl,
        _getPath
    );

    private static readonly string _fullPathGetAll = string.Concat(
        ItemInfoFetcher.ItemInfoBaseUrl,
        _getAllPath
    );
    private static readonly string _getPath = """1""";
    private static readonly string _getAllPath = string.Empty;
}
