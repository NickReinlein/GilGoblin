// using GilGoblin.Controllers;
// using GilGoblin.Pocos;
// using GilGoblin.Repository;
// using Microsoft.Extensions.Logging;
// using Microsoft.Extensions.Logging.Abstractions;
// using NSubstitute;
// using NUnit.Framework;

// namespace GilGoblin.Tests.Controllers;

// public class CraftControllerTests
// {
//     private CraftController? _controller;
//     private readonly ICraftRepository<CraftSummaryPoco> _repo = Substitute.For<
//         ICraftRepository<CraftSummaryPoco>
//     >();

//     private static readonly int _world = 34;
//     private static readonly int _craftId = 108;

//     [SetUp]
//     public void SetUp()
//     {
//         _controller = new CraftController(
//             _repo,
//             NullLoggerFactory.Instance.CreateLogger<CraftController>()
//         );
//     }

//     [TearDown]
//     public void TearDown()
//     {
//         _repo.ClearReceivedCalls();
//     }

//     [Test]
//     public void WhenWeSetup_ControllerIsSucessfullyCreated()
//     {
//         Assert.That(_controller, Is.Not.Null);
//     }

//     [Test]
//     public void WhenReceivingARequestGetBestCrafts_ThenTheRepositoryIsCalled()
//     {
//         _repo.GetBestCrafts(_world).Returns(new List<CraftSummaryPoco>());
//         _ = _controller!.GetBestCrafts(_world);

//         _repo.GetBestCrafts(_world);
//     }

//     [Test]
//     public void WhenReceivingARequestGetCraft_ThenTheRepositoryIsCalled()
//     {
//         _ = _controller!.GetCraft(_world, _craftId);

//         _repo.Received(1).GetCraft(_world, _craftId);
//     }

//     [Test]
//     public void WhenReceivingARequestGetBestCrafts_ThenTheAnEnumerableIsReturned()
//     {
//         _repo.GetBestCrafts(_world).Returns(new List<CraftSummaryPoco>());

//         var result = _controller!.GetBestCrafts(_world);

//         Assert.That(result is not null);
//     }

//     [Test]
//     public void WhenReceivingARequestGetCraft_ThenAPocoIsReturned()
//     {
//         _repo.GetCraft(_world, _craftId).Returns(new CraftSummaryPoco());

//         var result = _controller!.GetCraft(_world, _craftId);

//         Assert.That(result is not null);
//     }
// }
