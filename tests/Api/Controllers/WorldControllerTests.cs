using System.Collections.Generic;
using System.Linq;
using GilGoblin.Api.Controllers;
using GilGoblin.Api.Repository;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Api.Tests.Controllers
{
    [TestFixture]
    public class WorldControllerTests
    {
        private WorldController _worldController;
        private IWorldRepository _worldRepo;
        private ILogger<WorldController> _loggerMock;
        private Dictionary<int, string> _worlds;

        [SetUp]
        public void Setup()
        {
            _worldRepo = Substitute.For<IWorldRepository>();
            _loggerMock = Substitute.For<ILogger<WorldController>>();
            _worldController = new WorldController(_worldRepo, _loggerMock);

            _worlds = new Dictionary<int, string> { { 1, "World 1" }, { 2, "World 2" }, { 3, "World 3" } };
            _worldRepo.GetAllWorlds().Returns(_worlds);
        }

        [Test]
        public void GetAll_ReturnsCorrectDictionary()
        {
            var result = _worldController.GetAllWorlds();

            Assert.That(result, Is.EqualTo(_worlds));
        }

        [Test]
        public void Get_ReturnsCorrectKeyValuePair()
        {
            var world = _worlds.First();

            var result = _worldController.GetWorld(world.Key);

            Assert.That(result.Value, Is.EqualTo(world.Value));
        }
    }
}