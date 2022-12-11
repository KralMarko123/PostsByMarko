using Microsoft.Data.SqlClient;
using PostsTesting.Utility.Constants;

namespace PostsTesting.Utility
{
    public static class DbUtility
    {
        private static SqlConnection? connection;

        public static async Task<SqlDataReader> ExecuteQueryAsync(string query)
        {
            await InitializeConnectionAsync();

            var sqlCommand = connection.CreateCommand();
            sqlCommand.CommandText = query;

            return await sqlCommand.ExecuteReaderAsync();
        }

        public static async Task<int> ExecuteUpdateQueryAsync(string query)
        {
            await InitializeConnectionAsync();

            var sqlCommand = connection.CreateCommand();
            sqlCommand.CommandText = query;

            return await sqlCommand.ExecuteNonQueryAsync();
        }

        private static async Task InitializeConnectionAsync()
        {
            if (connection == null) await OpenConnectionAsync();
            else if (connection != null && connection.State != System.Data.ConnectionState.Open) await OpenConnectionAsync();
        }

        public static async Task OpenConnectionAsync()
        {
            connection = new SqlConnection(TestingConstants.connectionString);
            await connection.OpenAsync();
        }

        public static async Task CloseConnectionAsync()
        {
            await connection.CloseAsync();
            connection = null;
        }
    }
}
