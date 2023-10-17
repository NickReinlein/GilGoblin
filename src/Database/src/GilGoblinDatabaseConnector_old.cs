// using System;
// using System.Data;
// using System.IO;
// using Microsoft.Data.Sqlite;
// using Microsoft.Extensions.Logging;
//
// namespace GilGoblin.Database;
//
// public interface ISqlLiteDatabaseConnector
// {
//     public SqliteConnection? Connect();
//     public void Disconnect();
//     public string GetDatabasePath();
//     public string GetResourcesPath();
// }
//
// public class GilGoblinDatabaseConnector : ISqlLiteDatabaseConnector
// {
//     private readonly ILogger<GilGoblinDatabaseConnector> _logger;
//     public static SqliteConnection? Connection { get; set; }
//
//     public GilGoblinDatabaseConnector(ILogger<GilGoblinDatabaseConnector> logger)
//     {
//         _logger = logger;
//     }
//
//     public SqliteConnection? Connect() => InitiateConnection();
//
//     public void Disconnect()
//     {
//         Connection?.Close();
//         Connection?.Dispose();
//     }
//
//     private SqliteConnection InitiateConnection()
//     {
//         if (IsConnectionOpen)
//             return Connection;
//
//         try
//         {
//             var databasePath = GetDatabasePath();
//             var absolutePath = Path.GetFullPath(databasePath);
//             Connection = new SqliteConnection("Data Source=" + databasePath);
//             Connection.Open();
//             if (IsConnectionOpen)
//                 return Connection;
//
//             throw new Exception();
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError($"Failed to initiate a connection: {ex.Message}");
//             return null;
//         }
//     }
//
//     public string GetDatabasePath() => Path.Combine(GetResourcesPath(), DbFileName);
//     public string GetResourcesPath() => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../resources/");
//     public static bool IsConnectionOpen => Connection?.State == ConnectionState.Open;
//     public static readonly string DbFileName = "GilGoblin.db";
// }