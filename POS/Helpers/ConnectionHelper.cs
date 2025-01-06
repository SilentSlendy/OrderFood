using System;
using Npgsql;

namespace POS.Helpers
{
    /// <summary>
    /// Helper class để tạo connection string từ các biến môi trường
    /// </summary>
    public static class ConnectionHelper
    {
        /// <summary>
        /// Tạo connection string từ các biến môi trường
        /// </summary>
        /// <returns></returns>
        public static string BuildConnectionString()
        {
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = Environment.GetEnvironmentVariable("DB_HOST"),
                Port = int.Parse(Environment.GetEnvironmentVariable("DB_PORT")),
                Database = Environment.GetEnvironmentVariable("DB_NAME"),
                Username = Environment.GetEnvironmentVariable("DB_USER"),
                Password = Environment.GetEnvironmentVariable("DB_PASS"),
                IncludeErrorDetail = true
            };
            return builder.ConnectionString;
        }
    }
}
