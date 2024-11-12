using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Api.Cache;
using GilGoblin.Api.Repository;
using GilGoblin.Database.Pocos;
using GilGoblin.Tests.Database.Integration;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Api.Repository.Integration;

public class PriceRepositoryTests : GilGoblinDatabaseFixture
{
    private IPriceCache _cache;
    private PriceRepository _priceRepo;

    [SetUp]
    public override async Task SetUp()
    {
        _cache = Substitute.For<IPriceCache>();
        await base.SetUp();

        _priceRepo = new PriceRepository(_serviceProvider, _cache);
    }

    [Test]
    public void GivenAGetAll_WhenTheWorldIdExists_ThenTheRepositoryReturnsAllEntriesForThatWorld()
    {
        var result = _priceRepo.GetAll(ValidWorldIds[1]).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.GreaterThanOrEqualTo(2));
            Assert.That(result.All(x => x.WorldId == ValidWorldIds[1]));
            Assert.That(result.All(x => ValidItemsIds.Contains(x.ItemId)));
        });
    }

    [Test]
    public void GivenAGetAll_WhenTheWorldIdDoesNotExist_ThenAnEmptyResponseIsReturned()
    {
        var result = _priceRepo.GetAll(999123);

        Assert.That(!result.Any());
    }

    [TestCaseSource(nameof(GetValidItemIdsAndQuality))]
    public void GivenAGet_WhenTheIdIsValid_ThenTheRepositoryReturnsTheCorrectEntry(int id, bool quality)
    {
        var result = _priceRepo.Get(ValidWorldIds[0], id, quality);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ItemId, Is.EqualTo(id));
            Assert.That(result.WorldId, Is.EqualTo(ValidWorldIds[0]));
        });
    }

    [TestCaseSource(nameof(ValidItemsIds))]
    public void GivenAGet_WhenTheIdIsValidButNotTheWorldId_ThenNullIsReturned(int id)
    {
        var result = _priceRepo.Get(854, id, true);

        Assert.That(result, Is.Null);
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(100)]
    public void GivenAGet_WhenIdIsInvalid_ThenNullIsReturned(int id)
    {
        var result = _priceRepo.Get(ValidItemsIds[0], id, true);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void GivenAGetMultiple_WhenIdsAreValid_ThenTheCorrectEntriesAreReturned([Values] bool quality)
    {
        var result = _priceRepo.GetMultiple(ValidWorldIds[0], ValidItemsIds, quality).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.GreaterThanOrEqualTo(2));
            Assert.That(result.All(x => x.WorldId == ValidWorldIds[0]));
            Assert.That(result.All(x => ValidItemsIds.Contains(x.ItemId)));
            Assert.That(result.All(x => x.IsHq == quality));
        });
    }

    [Test]
    public void GivenAGetMultiple_WhenIdsAreValidButNotWorldId_ThenAnEmptyResponseIsReturned()
    {
        var result = _priceRepo.GetMultiple(6845454, ValidRecipeIds, true);

        Assert.That(!result.Any());
    }

    [Test]
    public void GivenAGetMultiple_WhenSomeIdsAreValid_ThenTheValidEntriesAreReturned()
    {
        const int invalidItemId = 99654;
        var validItemCount = ValidItemsIds.Count;
        var mixedValidityItemsIds = ValidItemsIds.ToList();
        mixedValidityItemsIds.Add(invalidItemId);

        var result = _priceRepo.GetMultiple(ValidWorldIds[0], mixedValidityItemsIds, true).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(validItemCount));
            Assert.That(result.All(r => r.ItemId != invalidItemId));
            Assert.That(result.All(r => r.WorldId == ValidWorldIds[0]));
            Assert.That(result.All(r => ValidItemsIds.Contains(r.ItemId)));
        });
    }

    [Test]
    public void GivenAGetMultiple_WhenIdsAreInvalid_ThenAnEmptyResponseIsReturned()
    {
        var result = _priceRepo.GetMultiple(ValidWorldIds[0], [654645646, 9953121], true);

        Assert.That(!result.Any());
    }

    [Test]
    public void GivenAGetMultiple_WhenIdsEmpty_ThenAnEmptyResponseIsReturned()
    {
        using var context = GetDbContext();

        var result = _priceRepo.GetMultiple(ValidWorldIds[0], Array.Empty<int>(), true);

        Assert.That(!result.Any());
    }

    #region Caching

    [Test]
    public void GivenAGet_WhenTheIdIsValidAndUncached_ThenWeCacheTheEntry()
    {
        using var context = GetDbContext();
        var validWorldId = ValidWorldIds[0];
        var validItemsId = ValidItemsIds[0];

        _ = _priceRepo.Get(validWorldId, validItemsId, true);

        _cache.Received(1).Get((validWorldId, validItemsId, true));
        _cache
            .Received(1)
            .Add((validWorldId, validItemsId, true),
                Arg.Is<PricePoco>(price =>
                    price.WorldId == validWorldId &&
                    price.ItemId == validItemsId));
    }

    [Test]
    public void GivenAGet_WhenTheIdIsValidAndCached_ThenWeReturnTheCachedEntry()
    {
        using var context = GetDbContext();
        var poco = context.Price.First();
        _cache.Get((poco.WorldId, poco.ItemId, true)).Returns(null, poco);

        _ = _priceRepo.Get(poco.WorldId, poco.ItemId, true);
        _ = _priceRepo.Get(poco.WorldId, poco.ItemId, true);

        _cache.Received(2).Get((poco.WorldId, poco.ItemId, true));
        _cache
            .Received(1)
            .Add((poco.WorldId, poco.ItemId, true),
                Arg.Is<PricePoco>(price =>
                    price.WorldId == poco.WorldId &&
                    price.ItemId == poco.ItemId &&
                    price.IsHq == poco.IsHq)
            );
    }

    [Test]
    public async Task GivenAFillCache_WhenEntriesExist_ThenWeFillTheCache()
    {
        await using var context = GetDbContext();
        var allPrices = context.Price.ToList();

        await _priceRepo.FillCache();

        allPrices.ForEach(price => _cache.Received(1).Add((price.WorldId, price.ItemId, price.IsHq), price));
    }

    [Test]
    public async Task GivenAFillCache_WhenEntriesDoNotExist_ThenWeDoNothing()
    {
        await using var context = GetDbContext();
        context.Price.RemoveRange(context.Price);
        await context.SaveChangesAsync();

        await _priceRepo.FillCache();

        _cache.DidNotReceive().Add(Arg.Any<(int, int, bool)>(), Arg.Any<PricePoco>());
    }

    #endregion Caching

    private static IEnumerable<TestCaseData> GetValidItemIdsAndQuality()
    {
        foreach (var id in ValidItemsIds)
        {
            yield return new TestCaseData(id, true);
            yield return new TestCaseData(id, false);
        }
    }
}