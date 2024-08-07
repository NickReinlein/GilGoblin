using System.Linq;
using GilGoblin.Api.Cache;
using NUnit.Framework;

namespace GilGoblin.Tests.Api.Cache;

public class DataCacheTests : DataCache<int, Pizza>
{
    [Test]
    public void GivenACallToAdd_WhenTheValuesAreValid_ThenTheyAreStored()
    {
        var pizza = new Pizza() { Id = 99, IngredientsCount = 999 };

        Add(pizza.Id, pizza);

        Assert.That(Cache[pizza.Id], Is.EqualTo(pizza));
    }

    [Test]
    public void GivenACallToGet_WhenTheValuesAreNew_ThenWeReturnNull()
    {
        var result = Get(999);

        Assert.That(result, Is.Null);
    }

    [TestCase(1)]
    [TestCase(2)]
    public void GivenACallToGet_WhenTheValuesExist_ThenTheyAreReturned(int id)
    {
        var result = Get(id);

        Assert.That(result!.Id, Is.EqualTo(id));
        Assert.That(result.IngredientsCount, Is.EqualTo(10 * id));
    }

    [TestCase(1)]
    [TestCase(2)]
    public void GivenACallToGetMultiple_WhenTheValuesExist_ThenTheyAreReturned(int id)
    {
        var result = Get(id);

        Assert.That(result!.Id, Is.EqualTo(id));
        Assert.That(result.IngredientsCount, Is.EqualTo(10 * id));
    }

    [Test]
    public void GivenACallToGetMultiple_WhenTheValuesAreAllNew_ThenWeReturnEmptyResponse()
    {
        var result = GetMultiple(new[] { 2000, 2001 });

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void GivenACallToGetMultiple_WhenTheValuesAllExist_ThenWeReturnThemAll()
    {
        var keys = Cache.Keys.ToList();

        var result = GetMultiple(keys);

        Assert.Multiple(() =>
        {
            Assert.That(result.Count, Is.GreaterThanOrEqualTo(3));
            Assert.That(result.Count, Is.EqualTo(keys.Count));
        });
    }

    [Test]
    public void GivenACallToGetMultiple_WhenSomeValuesExist_ThenWeReturnThem()
    {
        var keys = Cache.Keys.ToList();
        var realCount = keys.Count;
        keys.Add(30000);
        keys.Add(30001);

        var result = GetMultiple(keys);

        Assert.Multiple(() =>
        {
            Assert.That(realCount, Is.GreaterThanOrEqualTo(3));
            Assert.That(result.Count, Is.EqualTo(realCount));
            Assert.That(!result.Any(r => r.Id >= 30000));
        });
    }

    [Test]
    public void GivenACallToGetAll_WhenValuesExist_ThenWeReturnThemAll()
    {
        var result = GetAll();

        Assert.Multiple(() =>
        {
            Assert.That(result.Count, Is.GreaterThanOrEqualTo(3));
            Assert.That(result.Count, Is.EqualTo(Cache.Keys.Count));
        });
    }

    [Test]
    public void GivenACallToGetAll_WhenNoValuesExist_ThenWeReturnEmptyResponse()
    {
        Cache.Clear();

        var result = GetAll();

        Assert.That(result, Is.Empty);
    }

    [SetUp]
    public void SetUp()
    {
        Cache.Clear();
        Cache[1] = new Pizza { Id = 1, IngredientsCount = 10 };
        Cache[2] = new Pizza { Id = 2, IngredientsCount = 20 };
        Cache[3] = new Pizza { Id = 3, IngredientsCount = 30 };
    }
}

public record Pizza
{
    public int Id { get; set; }
    public int IngredientsCount { get; set; }
}