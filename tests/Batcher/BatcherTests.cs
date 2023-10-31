using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using GilGoblin.Batcher;

namespace GilGoblin.Tests.Batcher;

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
    public void GivenMultipleEntries_WhenWeSplitIntoJobsLessThanPageSize_WeReturnOneBatch()
    {
        var array = new List<int> { 1, 2, 3 };

        var result = _batcher.SplitIntoBatchJobs(array);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First(), Has.Count.EqualTo(3));
    }

    [Test]
    public void GivenMultipleEntries_WhenWeSplitIntoBatchesMoreThanPageSize_WeReturnMultipleBatches()
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