using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using GilGoblin.Tests.Database.Integration;
using GilGoblin.Api;
using GilGoblin.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using EFCore.NamingConventions;

namespace GilGoblin.Tests.Component;

[Category("Component")]
[Timeout(10000)]
public class TestBase : GilGoblinDatabaseFixture
{
    private TestServer _server = null!;
    protected HttpClient _client;

    [OneTimeSetUp]
    public override async Task OneTimeSetUp()
    {
        await base.OneTimeSetUp();
        
        var builder = new WebHostBuilder()
            .UseEnvironment("Testing")
            .UseConfiguration(_configuration)
            .UseStartup<Startup>();
        _server = new TestServer(builder);
        _client = _server.CreateClient();
    }

    [OneTimeTearDown]
    public override async Task OneTimeTearDown()
    {
        _client.Dispose();
        _server.Dispose();
        await base.OneTimeTearDown();
    }

    protected static JsonSerializerOptions GetSerializerOptions() =>
        new() { PropertyNameCaseInsensitive = true, IncludeFields = true };
}