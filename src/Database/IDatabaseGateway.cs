using Microsoft.Data.Sqlite;

namespace GilGoblin.Database
{
    public interface IDatabaseGateway
    {
        public SqliteConnection? Connect();
        public bool Disconnect();

    }
}