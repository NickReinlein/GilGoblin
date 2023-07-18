using GilGoblin.Pocos;
using GilGoblin.Services;
using NUnit.Framework;

namespace GilGoblin.Tests.Services;

public class CsvInteractorTests
{
    [SetUp]
    public void SetUp() { }

    [Test]
    public void GivenAnItemTestFile_WhenWeLoadTheFile_ThenWeDeserializeItems()
    {
        var result = CsvInteractor<ItemInfoPoco>.LoadFile(ResourceFilePath(ItemTestFileName));

        Assert.That(result.Count, Is.GreaterThan(10));
        var gilItemEntry = result.First(i => i.ID == 1);
        var lightningShardItemEntry = result.First(i => i.ID == 6);
        Assert.Multiple(() =>
        {
            Assert.That(gilItemEntry.Name, Is.EqualTo("Gil"));
            Assert.That(gilItemEntry.IconID, Is.EqualTo(65002));
            Assert.That(gilItemEntry.StackSize, Is.GreaterThanOrEqualTo(9999999));
            Assert.That(gilItemEntry.StackSize, Is.GreaterThanOrEqualTo(9999999));
            Assert.That(lightningShardItemEntry.Name, Is.EqualTo("Lightning Shard"));
            Assert.That(lightningShardItemEntry.IconID, Is.EqualTo(20005));
            Assert.That(lightningShardItemEntry.StackSize, Is.EqualTo(9999));
        });
    }

    [Test]
    public void GivenARecipesTestFile_WhenWeLoadTheFileThenWeDeserializeRecipes()
    {
        var result = CsvInteractor<RecipePoco>.LoadFile(ResourceFilePath(RecipeTestFileName));

        Assert.That(result.Count, Is.GreaterThan(10));
        var recipeSix = result.First<RecipePoco>(i => i.ID == 6);
        Assert.Multiple(() =>
        {
            Assert.That(recipeSix.ID, Is.EqualTo(6));
            Assert.That(recipeSix.TargetItemID, Is.EqualTo(1750));
            Assert.That(recipeSix.ResultQuantity, Is.EqualTo(1));
            Assert.That(recipeSix.ItemIngredient0TargetID, Is.EqualTo(5056));
            Assert.That(recipeSix.AmountIngredient0, Is.EqualTo(2));
            Assert.That(recipeSix.ItemIngredient1TargetID, Is.EqualTo(5361));
            Assert.That(recipeSix.AmountIngredient1, Is.EqualTo(1));
        });
    }

    [Test]
    public void GivenAPricesTestFile_WhenWeLoadTheFile_ThenWeDeserializePrices()
    {
        var result = CsvInteractor<PricePoco>.LoadFile(ResourceFilePath(PriceTestFileName));

        Assert.That(result.Count, Is.GreaterThan(10));
        var price = result.First<PricePoco>(i => i.ItemID == 31100);
        Assert.Multiple(() =>
        {
            Assert.That(price.ItemID, Is.EqualTo(31100));
            Assert.That(price.WorldID, Is.GreaterThan(0));
            Assert.That(price.AverageListingPrice, Is.GreaterThan(0));
            Assert.That(price.AverageSold, Is.GreaterThan(0));
            Assert.That(price.LastUploadTime, Is.GreaterThan(0));
        });
    }

    private static string ResourcesFolderPath = System.IO.Path.Combine(
        Directory
            .GetParent(System.IO.Directory.GetCurrentDirectory())
            .Parent.Parent.Parent.FullName,
        "resources/"
    );

    private static string ResourceFilePath(string filename) =>
        System.IO.Path.Combine(ResourcesFolderPath, filename);

    public const string ItemTestFileName = "ItemInfoTest.csv";
    public const string RecipeTestFileName = "RecipeTest.csv";
    public const string PriceTestFileName = "PriceTest.csv";
}
