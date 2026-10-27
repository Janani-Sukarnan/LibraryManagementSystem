
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Repository
{
    public class MemberRepository
    {
        private readonly string _connectionString;

        public MemberRepository()
        {
            _connectionString = DatabaseConfig.ConnectionString;
        }

        // Add a new member to SQL Server
        public async Task AddMemberAsync(Member member)
        {
            const string query = @"
                INSERT INTO Members
                    (MemberId, Name, ContactNumber, Email)
                VALUES
                    (@MemberId, @Name, @ContactNumber, @Email)";

            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@MemberId", member.MemberId);
            command.Parameters.AddWithValue("@Name", member.Name);
            command.Parameters.AddWithValue("@ContactNumber", member.ContactNumber);
            command.Parameters.AddWithValue("@Email", member.Email);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        // Get all members from SQL Server
        public async Task<List<Member>> GetAllMembersAsync()
        {
            const string query = @"
                SELECT MemberId, Name, ContactNumber, Email
                FROM Members
                ORDER BY MemberId";

            var members = new List<Member>();

            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand(query, connection);

            await connection.OpenAsync();

            await using var reader =
                await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                members.Add(new Member(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetString(3)
                ));
            }

            return members;
        }

        // Find a member by ID
        public async Task<Member?> GetMemberByIdAsync(int memberId)
        {
            const string query = @"
                SELECT MemberId, Name, ContactNumber, Email
                FROM Members
                WHERE MemberId = @MemberId";

            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@MemberId", memberId);

            await connection.OpenAsync();

            await using var reader =
                await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Member(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetString(3)
                );
            }

            return null;
        }
    }
}
