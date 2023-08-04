using Serilog;
using GilGoblin.Web;
using System.Threading.Tasks;
using System;
using GilGoblin.Extensions;

namespace GilGoblin.Database;

public class GilGoblinDatabase : IContextFetcher
{
    private readonly IPriceDataFetcher _priceFetcher;
    private readonly ISqlLiteDatabaseConnector _dbConnector;

    public GilGoblinDatabase(IPriceDataFetcher priceFetcher, ISqlLiteDatabaseConnector dbConnector)
    {
        _priceFetcher = priceFetcher;
        _dbConnector = dbConnector;
    }

    public async Task<GilGoblinDbContext?> GetContextAsync()
    {
        var connection = _dbConnector.Connect();
        if (!connection.IsOpen())
            return null;

        var dbContext = new GilGoblinDbContext();
        var databaseInitializer = new GilGoblinDatabaseInitializer(_priceFetcher, _dbConnector);
        await databaseInitializer.FillTablesIfEmpty(dbContext);
        return dbContext;
    }

    public static async Task Save()
    {
        try
        {
            using var context = new GilGoblinDbContext();
            var savedEntries = await context.SaveChangesAsync();
            Log.Debug("Saved {saved} entries to the database.", savedEntries);
        }
        catch (Exception ex)
        {
            Log.Error("Database save failed! {Message}.", ex.Message);
        }
    }
}
