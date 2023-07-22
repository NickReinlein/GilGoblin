using System;
using System.IO;
using Microsoft.Data.Sqlite;
using Serilog;
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
    public static SqliteConnection? Connection { get; set; }
    public string? ResourceDirectory { get; set; }

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
            ResourceDirectory ??= GetBaseDirectory();
            var path = Path.Combine(ResourceDirectory, DbFileName);

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
            Log.Error("Failed to initiate a connection: {Message}.", ex.Message);
            return null;
        }
    }

    public string GetDatabasePath() => Path.Combine(ResourcesFolderPath, DbFileName);

    public string GetBaseDirectory() =>
        ResourceDirectory ?? Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

    public string ResourcesFolderPath => Path.Combine(GetBaseDirectory(), "resources/");
    public static bool IsConnectionOpen => Connection.IsOpen();

    public static string ResourceFilenameCsv(string filename) => string.Concat(filename, ".csv");

    public static readonly string DbFileName = "GilGoblin.db";
}
