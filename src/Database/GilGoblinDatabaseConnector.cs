using System;
using System.IO;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using GilGoblin.Extensions;

namespace GilGoblin.Database;

public interface ISqlLiteDatabaseConnector
{
    public SqliteConnection? Connect();
    public void Disconnect();
    public string GetDatabasePath();
}

public class GilGoblinDatabaseConnector : ISqlLiteDatabaseConnector
{
    private readonly string _resourceDirectory;
    private readonly ILogger<GilGoblinDatabaseConnector> _logger;

    public static SqliteConnection? Connection { get; set; }

    public GilGoblinDatabaseConnector(
        ILogger<GilGoblinDatabaseConnector> logger,
        string? resourceDirectory = null
    )
    {
        _logger = logger;
        _resourceDirectory = resourceDirectory ?? GetBaseDirectory();
    }

    public SqliteConnection? Connect()
    {
        if (IsConnectionOpen)
            return Connection;

        return InitiateConnection();
    }

    public void Disconnect()
    {
        Connection?.Close();
        Connection?.Dispose();
    }

    private SqliteConnection InitiateConnection()
    {
        try
        {
            var path = Path.Combine(_resourceDirectory, DbFileName);

            Connection = new SqliteConnection("Data Source=" + path);
            if (IsConnectionOpen)
                return Connection;

            Connection.Open();
            if (IsConnectionOpen)
                return Connection;

            throw new Exception($"Connection not open. State is: {Connection?.State}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to initiate a connection: {ex.Message}");
            return null;
        }
    }

    public string GetBaseDirectory() =>
        _resourceDirectory ?? Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

    public string GetDatabasePath() => Path.Combine(GetResourcesFolderPath(), DbFileName);

    public string GetResourcesFolderPath() => Path.Combine(GetBaseDirectory(), "resources/");

    public static bool IsConnectionOpen => Connection.IsOpen();

    public static string ResourceFilenameCsv(string filename) => string.Concat(filename, ".csv");

    public static readonly string DbFileName = "GilGoblin.db";
}
