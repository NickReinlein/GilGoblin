using GilGoblin.Pocos;
using GilGoblin.Services;
using NUnit.Framework;

namespace GilGoblin.Tests.Services;

public class CsvInteractorTests
{
    [SetUp]
    public void SetUp() { }

    [Test]
    public void WhenWeLoadATestFile_ThenThereAreNoErrors()
    {
        Assert.DoesNotThrow(() =>
        {
            var result = CsvInteractor<ItemInfoPoco>.LoadFile(ResourceFilePath);
            Assert.NotNull(result);
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
