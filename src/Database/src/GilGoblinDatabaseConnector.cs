// using System;
// using System.Data;
// using System.Data.Common;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.Logging;
// using Npgsql;
//
// namespace GilGoblin.Database
// {
//     public interface IDatabaseConnector
//     {
//         public DbConnection Connect();
//         public void Disconnect();
//     }
//
//     public class GilGoblinDatabaseConnector : IDatabaseConnector
//     {
//         private readonly IConfiguration _configuration;
//         private readonly ILogger<GilGoblinDatabaseConnector> _logger;
//         public static DbConnection Connection { get; set; }
//
//         public GilGoblinDatabaseConnector(
//             IConfiguration configuration, 
//             ILogger<GilGoblinDatabaseConnector> logger)
//         {
//             _configuration = configuration;
//             _logger = logger;
//         }
//
//         public DbConnection Connect() => InitiateConnection();
//
//         public void Disconnect()
//         {
//             Connection?.Close();
//             Connection?.Dispose();
//         }
//
//         private DbConnection InitiateConnection()
//         {
//             if (IsConnectionOpen)
//                 return Connection;
//
//             try
//             {
//                 var connectionString = GetConnectionString();
//                 Connection = new NpgsqlConnection(connectionString);
//                 Connection.Open();
//                 if (IsConnectionOpen)
//                     return Connection;
//
//                 throw new Exception("Failed to open a connection.");
//             }
//             catch (Exception ex)
//             {
//                 _logger.LogError($"Failed to initiate a connection: {ex.Message}");
//                 return null;
//             }
//         }
//
//         private string GetConnectionString()
//         {
//             var connectionString = _configuration.GetConnectionString(nameof(GilGoblinDbContext));
//             return connectionString;
//         }
//
//         public static bool IsConnectionOpen => Connection?.State == ConnectionState.Open;
//     }
// }