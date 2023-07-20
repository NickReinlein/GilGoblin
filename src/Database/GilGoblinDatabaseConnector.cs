using System;
using System.IO;
using Microsoft.Data.Sqlite;
using Serilog;

namespace GilGoblin.Database;

public class GilGoblinDatabaseConnector
{
    public static SqliteConnection? Connection { get; set; }

    public static SqliteConnection? Connect()
    {
        if (Connection is not null)
            return Connection;

        try
        {
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

    private static readonly string _resourcesFolderPath = System.IO.Path.Combine(
        Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName,
        "resources/"
    );

    public static string ResourceFilePath(string resourceFilename) =>
        System.IO.Path.Combine(_resourcesFolderPath, resourceFilename);

    public static string ResourceFilenameCsv(string filename) => string.Concat(filename, ".csv");

    public static readonly string DbFileName = "GilGoblin.db";
}
