using Microsoft.Data.Sqlite;

namespace GilGoblin.Extensions;

public static class SqliteConnectionExtensions
{
    public static bool IsOpen(this SqliteConnection? connection) =>
        connection?.State == System.Data.ConnectionState.Open;
}
