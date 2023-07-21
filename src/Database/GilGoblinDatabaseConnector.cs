using System;
using System.IO;
using Microsoft.Data.Sqlite;
using Serilog;
using GilGoblin.Extensions;

namespace GilGoblin.Database;

public class GilGoblinDatabaseConnector
{
    public static SqliteConnection? Connection { get; set; }
    public static string? ResourceDirectory { get; set; } = null;

    public static SqliteConnection? Connect(string? resourceDirectory = null)
    {
        if (ConnectionIsOpen)
            return Connection;

        return InitiateConnection(resourceDirectory);
    }

    private static SqliteConnection InitiateConnection(string? resourceDirectory)
    {
        try
        {
            ResourceDirectory = resourceDirectory ?? GetBaseDirectory();
            var path = ResourceFilePath(DbFileName);

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

    public static bool ConnectionIsOpen => Connection.IsOpen();
}
