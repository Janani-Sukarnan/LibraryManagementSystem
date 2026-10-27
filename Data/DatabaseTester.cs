
using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace LibraryManagementSystem.Data
{
    public static class DatabaseTester
    {
        public static async Task TestConnectionAsync()
        {
            string connectionString = DatabaseConfig.ConnectionString;

            await using var connection =
                new SqlConnection(connectionString);

            await connection.OpenAsync();

            Console.WriteLine("SQL Server connection successful!");
            Console.WriteLine($"Connected database: {connection.Database}");
        }
    }
}
