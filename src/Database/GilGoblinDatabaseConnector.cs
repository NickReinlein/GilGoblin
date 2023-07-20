using System;
using System.IO;
using Microsoft.Data.Sqlite;
using Serilog;

namespace GilGoblin.Database;

public class GilGoblinDatabaseConnector
{
    public static SqliteConnection? Connection { get; set; }
    public static string? ResourceDirectory { get; set; } = null;

    public static SqliteConnection? Connect(string? resourceDirectory = null)
    {
        if (Connection is not null)
            return Connection;

        try
        {
            ResourceDirectory = resourceDirectory;
            var path = ResourceFilePath(DbFileName);
            Connection = new SqliteConnection("Data Source=" + path);

            if (Connection.State == System.Data.ConnectionState.Open)
                return Connection;

            Connection.Open();
            if (Connection.State == System.Data.ConnectionState.Open)
                return Connection;

            throw new Exception($"Connection not open. State is: {Connection?.State}");
        }
        catch (Exception ex)
        {
            Log.Error("failed connection:{Message}.", ex.Message);
            return null;
        }
    }

    public static void Disconnect()
    {
        Connection?.Close();
        Connection?.Dispose();
    }

    public static string GetBaseDirectory() =>
        ResourceDirectory ?? Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

    public static string ResourcesFolderPath =>
        System.IO.Path.Combine(GetBaseDirectory(), "resources/");

    public static string ResourceFilePath(string resourceFilename) =>
        System.IO.Path.Combine(ResourcesFolderPath, resourceFilename);

    public static string ResourceFilenameCsv(string filename) => string.Concat(filename, ".csv");

    public static readonly string DbFileName = "GilGoblin.db";
}
