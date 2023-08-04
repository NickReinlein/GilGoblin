using GilGoblin.Database;
using GilGoblin.Repository;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Database;

public class ItemGatewayTests
{
    private ItemGateway _itemGateway;
    private IItemRepository _recipes;

    [SetUp]
    public void SetUp()
    {
        _recipes = Substitute.For<IItemRepository>();

        _itemGateway = new ItemGateway(_recipes);
    }

    [Test]
    public async Task GivenAGet_ThenTheRepositoryGetIsCalled()
    {
        await _itemGateway.Get(1);

        await _recipes.Received(1).Get(1);
    }

    [Test]
    public async Task GivenAGetMultiple_ThenTheRepositoryGetMultipleIsCalled()
    {
        var multiple = Enumerable.Range(1, 10);
        await _itemGateway.GetMultiple(multiple);

        await _recipes.Received(1).GetMultiple(multiple);
    }

    [Test]
    public async Task GivenAGetGetAll_ThenTheRepositoryGetAllIsCalled()
    {
        await _itemGateway.GetAll();

        await _recipes.Received(1).GetAll();
    }
}
