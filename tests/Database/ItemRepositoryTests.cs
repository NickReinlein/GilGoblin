// using GilGoblin.Database;
// using GilGoblin.Repository;
// using NSubstitute;
// using NUnit.Framework;

// namespace GilGoblin.Tests.Database;

// public class ItemRepositoryTests
// {
//     private ItemRepository _itemGateway;
//     private IItemRepository _recipes;

//     [SetUp]
//     public void SetUp()
//     {
//         _recipes = Substitute.For<IItemRepository>();

//         _itemGateway = new ItemRepository(_recipes);
//     }

//     [Test]
//     public async Task GivenAGet_ThenTheRepositoryGetIsCalled()
//     {
//         _itemGateway.Get(1);

//         _recipes.Received(1).Get(1);
//     }

//     [Test]
//     public async Task GivenAGetMultiple_ThenTheRepositoryGetMultipleIsCalled()
//     {
//         var multiple = Enumerable.Range(1, 10);
//         _itemGateway.GetMultiple(multiple);

//         _recipes.Received(1).GetMultiple(multiple);
//     }

//     [Test]
//     public async Task GivenAGetGetAll_ThenTheRepositoryGetAllIsCalled()
//     {
//         _itemGateway.GetAll();

//         _recipes.Received(1).GetAll();
//     }
// }
