// using System;
// using System.Collections.Generic;
// using System.Threading;
// using System.Threading.Tasks;
// using GilGoblin.Accountant;
// using GilGoblin.Api.Crafting;
// using GilGoblin.Api.Repository;
// using GilGoblin.Database;
// using GilGoblin.Database.Pocos;
// using GilGoblin.Tests.InMemoryTest;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Logging;
// using NSubstitute;
// using NUnit.Framework;
//
// namespace GilGoblin.Tests.Accountant;
//
// public class AccountantTests : InMemoryTestDb
// {
//     private Accountant<RecipeCostPoco> _accountant;
//     private ILogger<Accountant<RecipeCostPoco>> _logger;
//
//     private IServiceScopeFactory _scopeFactory;
//     private IServiceScope _scope;
//     private IServiceProvider _serviceProvider;
//     private GilGoblinDbContext _dbContext;
//     private ICraftingCalculator _calc;
//     private IRecipeCostRepository _recipeCostRepo;
//     private const int worldId = 34;
//
//     [SetUp]
//     public override void SetUp()
//     {
//         base.SetUp();
//         _dbContext = new GilGoblinDbContext(_options, _configuration);
//         _scopeFactory = Substitute.For<IServiceScopeFactory>();
//         _logger = Substitute.For<ILogger<Accountant<RecipeCostPoco>>>();
//         _scope = Substitute.For<IServiceScope>();
//         _serviceProvider = Substitute.For<IServiceProvider>();
//         _calc = Substitute.For<ICraftingCalculator>();
//         _recipeCostRepo = Substitute.For<IRecipeCostRepository>();
//
//         _scopeFactory.CreateScope().Returns(_scope);
//         _scope.ServiceProvider.Returns(_serviceProvider);
//         _serviceProvider.GetService(typeof(GilGoblinDbContext)).Returns(_dbContext);
//         _serviceProvider.GetService(typeof(ICraftingCalculator)).Returns(_calc);
//         _serviceProvider.GetService(typeof(IRecipeCostRepository)).Returns(_recipeCostRepo);
//
//         _accountant = new Accountant<RecipeCostPoco>(_scopeFactory, _logger);
//     }
//
//     [Test]
//     public void GivenComputeListAsync_WhenMethodIsNotImplemented_ThenWeThrowAnException()
//     {
//         Assert.ThrowsAsync<NotImplementedException>(async () =>
//             await _accountant.ComputeListAsync(worldId, new List<int> { 1, 2 }, CancellationToken.None));
//     }
//
//     [Test]
//     public void GivenComputeAsync_WhenMethodIsNotImplemented_ThenWeThrowAnException()
//     {
//         Assert.ThrowsAsync<NotImplementedException>(async () =>
//             await _accountant.ComputeAsync(worldId, 1, _calc));
//     }
//
//     [Test]
//     public async Task GivenGetIdsToUpdate_WhenMethodIsNotImplemented_ThenWeThrowAnException()
//     {
//         var ids = await _accountant.GetIdsToUpdate(1);
//
//         Assert.That(ids, Is.Empty);
//     }
// }