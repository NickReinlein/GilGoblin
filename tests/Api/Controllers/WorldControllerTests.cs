using System.Collections.Generic;
using System.Linq;
using GilGoblin.Api.Controllers;
using GilGoblin.Api.Repository;
using GilGoblin.Database.Pocos;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Api.Controllers
{
    [TestFixture]
    public class WorldControllerTests
    {
        private WorldController _worldController;
        private IWorldRepository _worldRepo;
        private ILogger<WorldController> _loggerMock;
        private List<WorldPoco> _worlds;

        [SetUp]
        public void Setup()
        {
            _worldRepo = Substitute.For<IWorldRepository>();
            _loggerMock = Substitute.For<ILogger<WorldController>>();
            _worldController = new WorldController(_worldRepo, _loggerMock);

            _worlds = new List<WorldPoco>
            {
                new() { Id = 1, Name = "World 1" },
                new() { Id = 2, Name = "World 2" },
                new() { Id = 3, Name = "World 3" }
            };
            _worldRepo.GetAll().Returns(_worlds);
            _worldRepo.Get(1).Returns(_worlds.First());
        }

        [Test]
        public void WhenGetAll_ThenAllWorldsAreReturned()
        {
            var result = _worldController.GetAllWorlds();

            Assert.That(result, Is.EqualTo(_worlds));
        }

        [Test]
        public void WhenGet_ThenTheCorrectWorldIsReturned()
        {
            var world = _worlds.First();

            var result = _worldController.GetWorld(world.Id).Value;

            Assert.That(result, Is.EqualTo(world));
        }
    }
}