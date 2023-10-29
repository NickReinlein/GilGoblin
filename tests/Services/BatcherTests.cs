using GilGoblin.Services;
using NUnit.Framework;

namespace GilGoblin.Tests.Services;

public class BatcherTests
{
    private Batcher<int> _batcher;

    [SetUp]
    public void SetUp()
    {
        _batcher = new Batcher<int>();
    }

    [Test]
    public void GivenAnEmptyList_WhenWeSplitIntoBatchJobs_WeReturnAnEmptyResult()
    {
        var result = _batcher.SplitIntoBatchJobs(new List<int>());

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Any(), Is.False);
    }

    [Test]
    public void GivenASingleEntry_WhenWeSplitIntoBatchJobs_WeReturnAListWithThatEntry()
    {
        var array = new List<int> { 1 };

        var result = _batcher.SplitIntoBatchJobs(array);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First(), Has.Count.EqualTo(1));
    }


    [Test]
    public void GivenTwoEntries_WhenWeSplitIntoBatchJobs_WeReturnAListWithTwoBatchesOfOneEntry()
    {
        var array = new List<int> { 1, 2 };

        var result = _batcher.SplitIntoBatchJobs(array);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.All(r => r.Count == 1));
    }

    [Test]
    public void GivenMultipleEntries_WhenWeSplitIntoBatchJobsLessThanPageSize_WeReturnInOneElement()
    {
        var array = new List<int> { 1, 2, 3 };

        var result = _batcher.SplitIntoBatchJobs(array);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0], Has.Count.EqualTo(1));
        Assert.That(result[1], Has.Count.EqualTo(2));
        var totalResult = result[0].Concat(result[1]).ToList();
        foreach (var id in array)
            Assert.That(totalResult, Does.Contain(id));
    }

    [Test]
    public void GivenMultipleEntries_WhenWeSplitIntoBatchJobsMoreThanPageSize_WeReturnMultipleElements()
    {
        _batcher = new Batcher<int>(5);
        var array = Enumerable.Range(1, 100).ToList();

        var result = _batcher.SplitIntoBatchJobs(array);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(20));
        Assert.That(result.First(), Has.Count.EqualTo(5));
        Assert.That(result.First().First(), Is.GreaterThanOrEqualTo(1));
    }
}