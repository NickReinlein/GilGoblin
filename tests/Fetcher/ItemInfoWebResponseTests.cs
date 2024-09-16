using System.Collections.Generic;
using System.Linq;
using GilGoblin.Database.Pocos;
using GilGoblin.Fetcher.Pocos;
using NUnit.Framework;

namespace GilGoblin.Tests.Fetcher;

public class ItemWebResponseTests
{
    public List<ItemWebPoco> _pocos = new();

    [SetUp]
    public void SetUp()
    {
        _pocos = GetMultipleNewPocos();
    }

    [Test]
    public void GivenANewItemWebResponse_WhenADictionaryIsProvided_ThenWeStoreEntriesCorrectly()
    {
        var dict = _pocos.ToDictionary(l => l.Id);

        var result = new ItemWebResponse(dict);

        Assert.Multiple(() =>
        {
            Assert.That(result.Items is not null);
            Assert.That(result.Items!.Count, Is.EqualTo(_pocos.Count));
            Assert.That(result.Items[_itemId1], Is.EqualTo(_pocos[0]));
            Assert.That(result.Items[_itemId2], Is.EqualTo(_pocos[1]));
        });
    }

    [Test]
    public void GivenANewItemWebResponse_WhenAnEmptyDictionaryIsProvided_ThenWeHaveEmptyResponseContent()
    {
        var dict = new Dictionary<int, ItemWebPoco>();

        var result = new ItemWebResponse(dict);

        Assert.Multiple(() =>
        {
            Assert.That(result is not null);
            Assert.That(result.Items is not null);
            Assert.That(result.Items, Is.Empty);
        });
    }

    [Test]
    public void GivenGetContentAsList_WhenADictionaryIsProvided_ThenWeStoreEntriesCorrectly()
    {
        var dict = _pocos.ToDictionary(l => l.Id);
        var response = new ItemWebResponse(dict);

        var content = response.GetContentAsList();

        Assert.Multiple(() =>
        {
            Assert.That(content.Count, Is.EqualTo(dict.Count));
            Assert.That(content.Exists(i => i.GetId() == _itemId1));
            Assert.That(content.Exists(i => i.GetId() == _itemId2));
        });
    }

    [Test]
    public void GivenANewItemWebResponse_WhenDatabaseListIsProvided_ThenWeStoreEntriesCorrectly()
    {
        var poco1 = new ItemPoco() { Id = _itemId1 };
        var poco2 = new ItemPoco { Id = _itemId2 };
        var pocoList = new List<ItemPoco> { poco1, poco2 };

        var result = new ItemWebResponse(pocoList);

        Assert.Multiple(() =>
        {
            Assert.That(result.Items.Count, Is.EqualTo(_pocos.Count));
            Assert.That(result.Items[_itemId1].Id, Is.EqualTo(_itemId1));
            Assert.That(result.Items[_itemId2].Id, Is.EqualTo(_itemId2));
        });
    }

    [Test]
    public void GivenANewItemWebResponse_WhenAnEmptyDatabaseListIsProvided_ThenWeStoreEntriesCorrectly()
    {
        var result = new ItemWebResponse(new List<ItemPoco>());

        Assert.Multiple(() =>
        {
            Assert.That(result is not null);
            Assert.That(result.Items is not null);
            Assert.That(result.Items, Is.Empty);
        });
    }

    [Test]
    public void GivenGetContentAsList_WhenDatabaseListIsProvided_ThenWeReturnEntriesCorrectly()
    {
        var poco1 = new ItemPoco() { Id = _itemId1 };
        var poco2 = new ItemPoco { Id = _itemId2 };
        var pocoList = new List<ItemPoco> { poco1, poco2 };
        var response = new ItemWebResponse(pocoList);

        var content = response.GetContentAsList();

        Assert.Multiple(() =>
        {
            Assert.That(content.Count, Is.EqualTo(pocoList.Count));
            Assert.That(content.Exists(i => i.GetId() == _itemId1));
            Assert.That(content.Exists(i => i.GetId() == _itemId2));
        });
    }

    protected static List<ItemWebPoco> GetMultipleNewPocos()
    {
        var poco1 = new ItemWebPoco { Id = _itemId1 };
        var poco2 = new ItemWebPoco { Id = _itemId2 };
        return new List<ItemWebPoco> { poco1, poco2 };
    }

    private static readonly int _itemId1 = 65711;
    private static readonly int _itemId2 = 86984;
}