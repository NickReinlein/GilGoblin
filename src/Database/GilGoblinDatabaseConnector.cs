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
}

public class GilGoblinDatabaseConnector : ISqlLiteDatabaseConnector
{
    public static SqliteConnection? Connection { get; set; }
    public string? ResourceDirectory { get; set; }

    public SqliteConnection? Connect()
    {
        if (ConnectionIsOpen)
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
            var path = ResourceFilePath(ResourceDirectory, DbFileName);

            Connection = new SqliteConnection("Data Source=" + path);
            if (ConnectionIsOpen)
                return Connection;

            Connection.Open();
            if (ConnectionIsOpen)
                return Connection;

            throw new Exception($"Connection not open. State is: {Connection?.State}");
        }
        catch (Exception ex)
        {
            Log.Error("Failed connection:{Message}.", ex.Message);
            return null;
        }
    }

    public static bool ConnectionIsOpen => Connection.IsOpen();

    public string GetBaseDirectory() =>
        ResourceDirectory ?? Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

    public string ResourcesFolderPath => System.IO.Path.Combine(GetBaseDirectory(), "resources/");

    public static string ResourceFilePath(string path, string filename) =>
        System.IO.Path.Combine(path, filename);

    public static string ResourceFilenameCsv(string filename) => string.Concat(filename, ".csv");

    public static readonly string DbFileName = "GilGoblin.db";
}
