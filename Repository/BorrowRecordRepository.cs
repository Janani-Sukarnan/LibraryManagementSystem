
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Repository
{
    public class BorrowRecordRepository
    {
        private readonly string _connectionString;

        public BorrowRecordRepository()
        {
            _connectionString = DatabaseConfig.ConnectionString;
        }

        // Borrow a book using a SQL transaction
        public async Task<int> BorrowBookAsync(int bookId, int memberId)
        {
            await using var connection =
                new SqlConnection(_connectionString);

            await connection.OpenAsync();

            await using var transaction =
                (SqlTransaction)await connection.BeginTransactionAsync();

            try
            {
                // Check whether the member exists
                const string memberQuery = @"
                    SELECT COUNT(*)
                    FROM Members
                    WHERE MemberId = @MemberId";

                await using var memberCommand =
                    new SqlCommand(memberQuery, connection, transaction);

                memberCommand.Parameters.AddWithValue("@MemberId", memberId);

                int memberExists = Convert.ToInt32(
                    await memberCommand.ExecuteScalarAsync());

                if (memberExists == 0)
                    throw new InvalidOperationException("Member not found.");

                // Mark the book unavailable only if it is available
                const string bookQuery = @"
                    UPDATE Books
                    SET IsAvailable = 0
                    WHERE BookId = @BookId
                      AND IsAvailable = 1";

                await using var bookCommand =
                    new SqlCommand(bookQuery, connection, transaction);

                bookCommand.Parameters.AddWithValue("@BookId", bookId);

                int bookUpdated = await bookCommand.ExecuteNonQueryAsync();

                if (bookUpdated == 0)
                {
                    const string checkBookQuery = @"
                        SELECT COUNT(*)
                        FROM Books
                        WHERE BookId = @BookId";

                    await using var checkBookCommand =
                        new SqlCommand(checkBookQuery, connection, transaction);

                    checkBookCommand.Parameters.AddWithValue("@BookId", bookId);

                    int bookExists = Convert.ToInt32(
                        await checkBookCommand.ExecuteScalarAsync());

                    if (bookExists == 0)
                        throw new InvalidOperationException("Book not found.");

                    throw new InvalidOperationException(
                        "This book is already borrowed.");
                }

                // Create the borrowing record
                const string borrowQuery = @"
                    INSERT INTO BorrowRecords (BookId, MemberId)
                    OUTPUT INSERTED.BorrowId
                    VALUES (@BookId, @MemberId)";

                await using var borrowCommand =
                    new SqlCommand(borrowQuery, connection, transaction);

                borrowCommand.Parameters.AddWithValue("@BookId", bookId);
                borrowCommand.Parameters.AddWithValue("@MemberId", memberId);

                int borrowId = Convert.ToInt32(
                    await borrowCommand.ExecuteScalarAsync());

                await transaction.CommitAsync();

                return borrowId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Return a borrowed book
        public async Task ReturnBookAsync(int borrowId)
        {
            await using var connection =
                new SqlConnection(_connectionString);

            await connection.OpenAsync();

            await using var transaction =
                (SqlTransaction)await connection.BeginTransactionAsync();

            try
            {
                // Find an active borrowing record
                const string findQuery = @"
                    SELECT BookId
                    FROM BorrowRecords WITH (UPDLOCK, ROWLOCK)
                    WHERE BorrowId = @BorrowId
                      AND ReturnDate IS NULL";

                int bookId;

                await using (var findCommand =
                    new SqlCommand(findQuery, connection, transaction))
                {
                    findCommand.Parameters.AddWithValue("@BorrowId", borrowId);

                    object? result = await findCommand.ExecuteScalarAsync();

                    if (result == null)
                        throw new InvalidOperationException(
                            "Active borrowing record not found. The book may already be returned.");

                    bookId = Convert.ToInt32(result);
                }

                // Set the return date
                const string returnQuery = @"
                    UPDATE BorrowRecords
                    SET ReturnDate = SYSDATETIME()
                    WHERE BorrowId = @BorrowId";

                await using (var returnCommand =
                    new SqlCommand(returnQuery, connection, transaction))
                {
                    returnCommand.Parameters.AddWithValue("@BorrowId", borrowId);
                    await returnCommand.ExecuteNonQueryAsync();
                }

                // Make the book available again
                const string bookQuery = @"
                    UPDATE Books
                    SET IsAvailable = 1
                    WHERE BookId = @BookId";

                await using (var bookCommand =
                    new SqlCommand(bookQuery, connection, transaction))
                {
                    bookCommand.Parameters.AddWithValue("@BookId", bookId);
                    await bookCommand.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Display all borrowing records
        public async Task<List<BorrowRecord>> GetAllBorrowRecordsAsync()
        {
            const string query = @"
                SELECT BorrowId, BookId, MemberId, BorrowDate, ReturnDate
                FROM BorrowRecords
                ORDER BY BorrowId";

            var records = new List<BorrowRecord>();

            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand(query, connection);

            await connection.OpenAsync();

            await using var reader =
                await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var record = new BorrowRecord(
                    reader.GetInt32(0),
                    reader.GetInt32(1),
                    reader.GetInt32(2),
                    reader.GetDateTime(3))
                {
                    ReturnDate = reader.IsDBNull(4)
                        ? null
                        : reader.GetDateTime(4)
                };

                records.Add(record);
            }

            return records;
        }
    }
}

