using Microsoft.Data.Sqlite;

namespace Sqlite.Services
{
    public static class SqliteService
    {
        public static SqliteConnection Open(string path)
        {
            var connectionString = $"Data Source={path}";

            var connection = new SqliteConnection(connectionString);

            connection.Open();

            return connection;
        }
    }
}