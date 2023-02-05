using GilGoblin.Pocos;
using GilGoblin.Services;
using NUnit.Framework;

namespace GilGoblin.Tests.Services;

public class CsvInteractorTests
{
    [SetUp]
    public void SetUp() { }

    [Test]
    public void WhenWeLoadATestFile_ThenWeReadAndDeserializeObjects()
    {
        var result = CsvInteractor<ItemInfoPoco>.LoadFile(ResourceFilePath);
        Assert.That(result.Count, Is.GreaterThan(10));
    }

    [Test]
    public void WhenWeLoadATestFile_ThenDeserializeObjectsCorrectly()
    {
        var result = CsvInteractor<ItemInfoPoco>.LoadFile(ResourceFilePath);
        var gilItemEntry = result.First(i => i.ID == 1);
        var lightningShardItemEntry = result.First(i => i.ID == 6);
        Assert.Multiple(() =>
        {
            Assert.That(gilItemEntry.Name, Is.EqualTo("Gil"));
            Assert.That(gilItemEntry.IconID, Is.EqualTo(65002));
            Assert.That(gilItemEntry.StackSize, Is.GreaterThanOrEqualTo(9999999));
            Assert.That(lightningShardItemEntry.Name, Is.EqualTo("Lightning Shard"));
            Assert.That(lightningShardItemEntry.IconID, Is.EqualTo(20005));
            Assert.That(lightningShardItemEntry.StackSize, Is.EqualTo(9999));
        });
    }

    public static readonly string ResourcesFolderPath = System.IO.Path.Combine(
        Directory
            .GetParent(System.IO.Directory.GetCurrentDirectory())
            .Parent.Parent.Parent.FullName,
        "resources/"
    );
    public static readonly string ResourceFilePath = System.IO.Path.Combine(
        ResourcesFolderPath,
        ResourceFileName
    );
    public const string ResourceFileName = "ItemTest.csv";
}
