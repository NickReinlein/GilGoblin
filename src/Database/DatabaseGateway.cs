using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database;

public class DatabaseGateway : IDatabaseGateway
{
    private readonly ILogger<DatabaseGateway> _log;
    public static readonly string file_path = Path.GetDirectoryName(AppContext.BaseDirectory);
    public static readonly string _db_name = "GilGoblin.db";
    public static readonly string _path = Path.Combine(this File_path, _db_name);
    public static SqliteConnection? Connection { get; set; }

    private readonly ILogger<DatabaseGateway> _log;
    public static string _file_path = Path.GetDirectoryName(AppContext.BaseDirectory);
    public static string _db_name = "GilGoblin.db";
    public static string _path = Path.Combine(_file_path, _db_name);
    public static SqliteConnection? Connection { get; set; }
    // public static MarketDataDB getContext()
    // {
    //     return new ItemDBContext();
    // }

    public DatabaseGateway()
    {

    }

    public SqliteConnection? Connect()
    {
        if (Connection is not null)
            return Connection;

        try
        {
            Connection = new SqliteConnection("Data Source=" + _path);

            //Already open, return
            if (Connection.State == System.Data.ConnectionState.Open)
                return Connection;

            Connection.Open();
            if (Connection.State == System.Data.ConnectionState.Open)
                return Connection;

            throw new Exception("Failed to connect to the database");
        }
        catch (Exception ex)
        {
            _log.LogError("failed connection:{message}.", ex.Message);
            return null;
        }
    }

    public bool Disconnect()
    {

        if (Connection is null || Connection.State == System.Data.ConnectionState.Closed)
            return true;

        try
        {
            Connection.Close();
            Connection.Dispose();
            _log.LogDebug("Sucessfully closed connection to database.");
            return true;
        }
        catch (Exception ex)
        {
            _log.LogError(ex.Message);
            return false;
        }

    }
}