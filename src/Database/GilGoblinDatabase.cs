using Serilog;
using GilGoblin.Services;
using GilGoblin.Pocos;
using GilGoblin.Web;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Net.Http;
using System.Collections.Generic;
using GilGoblin.Extensions;

namespace GilGoblin.Database;

public class GilGoblinDatabase : IContextFetcher
{
    private readonly IPriceDataFetcher _priceFetcher;
    private readonly ISqlLiteDatabaseConnector _dbConnector;
    private readonly GilGoblinDatabaseInitializer _databaseInitializer;

    private static GilGoblinDbContext _dbContext;

    public GilGoblinDatabase(IPriceDataFetcher priceFetcher, ISqlLiteDatabaseConnector dbConnector)
    {
        _priceFetcher = priceFetcher;
        _dbConnector = dbConnector;
        _databaseInitializer = new GilGoblinDatabaseInitializer(
            _dbContext,
            _priceFetcher,
            _dbConnector
        );
    }

    public async Task<GilGoblinDbContext?> GetContextAsync() => _dbContext ?? await GetNewContext();

    private async Task<GilGoblinDbContext?> GetNewContext()
    {
        var connection = _dbConnector.Connect();
        if (!connection.IsOpen())
            return null;

        _dbContext = new GilGoblinDbContext();
        await _databaseInitializer.FillTablesIfEmpty();
        return _dbContext;
    }

    public static async Task Save()
    {
        try
        {
            using var context = _dbContext;
            var savedEntries = await context.SaveChangesAsync();
            Log.Debug("Saved {saved} entries to the database.", savedEntries);
        }
        catch (Exception ex)
        {
            Log.Error("Database save failed! {Message}.", ex.Message);
        }
    }
}
