using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database;

public class DatabaseGateway : IDatabaseGateway
{
    private readonly ILogger<DatabaseGateway> _log;
    public static readonly string File_path = System.IO.Path.GetDirectoryName(AppContext.BaseDirectory) ?? string.Empty;
    public static readonly string Db_name = "GilGoblin.db";
    public static readonly string Path = System.IO.Path.Combine(File_path ?? string.Empty, Db_name);
    public static SqliteConnection? Connection { get; set; }

    public DatabaseGateway(ILogger<DatabaseGateway> log)
    {
        _log = log;
    }

    public SqliteConnection? Connect()
    {
        if (Connection is not null)
            return Connection;

        try
        {
            Connection = new SqliteConnection("Data Source=" + Path);

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
            _log.LogError("Failed to close the connection. Error Message: {ErrorMessage}", ex.Message);
            return false;
        }

    }
}