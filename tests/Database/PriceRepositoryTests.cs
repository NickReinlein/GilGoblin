// using GilGoblin.Database;
// using GilGoblin.Pocos;
// using GilGoblin.Repository;
// using NSubstitute;
// using NUnit.Framework;

// namespace GilGoblin.Tests.Database;

// public class PriceRepositoryTests
// {
//     private PriceRepository _priceGateway;
//     private GilGoblinDbContext _context;

//     [SetUp]
//     public void SetUp()
//     {
//         _context = new GilGoblinDbContext();
//         _context.Price.Add(new PricePoco { ItemID = 1, WorldID = 2 });
//         _context.SaveChanges();

//         _priceGateway = new PriceRepository(_context);
//     }

//     [Test]
//     public void GivenAGet_ThenTheRepositoryGetIsCalled()
//     {
//         var result = _priceGateway.Get(1, 2);

//         Assert.That(result, Is.Not.Null);
//         // _context.Price.Received(1).FirstOrDefault<PricePoco>();
//     }

//     // [Test]
//     // public async Task GivenAGetMultiple_ThenTheRepositoryGetMultipleIsCalled()
//     // {
//     //     var multiple = Enumerable.Range(1, 10);
//     //     await _priceGateway.GetMultiple(22, multiple);

//     //     await _context.Received(1).GetMultiple(22, multiple);
//     // }

//     // [Test]
//     // public async Task GivenAGetGetAll_ThenTheRepositoryGetAllIsCalled()
//     // {
//     //     await _priceGateway.GetAll(33);

//     //     await _context.Received(1).GetAll(33);
//     // }
// }
