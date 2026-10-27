
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Repository
{
    public class BookRepository
    {
        private readonly string _connectionString;

        public BookRepository()
        {
            _connectionString = DatabaseConfig.ConnectionString;
        }

        // Add a new book
        public async Task AddBookAsync(Book book)
        {
            const string query = @"
                INSERT INTO Books
                    (BookId, Title, Author, Category, Price, IsAvailable)
                VALUES
                    (@BookId, @Title, @Author, @Category, @Price, @IsAvailable)";

            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@BookId", book.BookId);
            command.Parameters.AddWithValue("@Title", book.Title);
            command.Parameters.AddWithValue("@Author", book.Author);
            command.Parameters.AddWithValue("@Category", book.Category);
            command.Parameters.AddWithValue("@Price", book.Price);
            command.Parameters.AddWithValue("@IsAvailable", book.IsAvailable);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        // Retrieve all books
        public async Task<List<Book>> GetAllBooksAsync()
        {
            const string query = @"
                SELECT BookId, Title, Author, Category, Price, IsAvailable
                FROM Books
                ORDER BY BookId";

            var books = new List<Book>();

            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand(query, connection);

            await connection.OpenAsync();

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                books.Add(new Book(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetString(3),
                    reader.GetDecimal(4))
                {
                    IsAvailable = reader.GetBoolean(5)
                });
            }

            return books;
        }

        // Search a book by ID
        public async Task<Book?> GetBookByIdAsync(int id)
        {
            const string query = @"
                SELECT BookId, Title, Author, Category, Price, IsAvailable
                FROM Books
                WHERE BookId = @BookId";

            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@BookId", id);

            await connection.OpenAsync();

            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Book(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetString(3),
                    reader.GetDecimal(4))
                {
                    IsAvailable = reader.GetBoolean(5)
                };
            }

            return null;
        }

        // Update book availability
        public async Task UpdateBookAvailabilityAsync(
            int bookId, bool isAvailable)
        {
            const string query = @"
                UPDATE Books
                SET IsAvailable = @IsAvailable
                WHERE BookId = @BookId";

            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@BookId", bookId);
            command.Parameters.AddWithValue("@IsAvailable", isAvailable);

            await connection.OpenAsync();

            int rowsAffected = await command.ExecuteNonQueryAsync();

            if (rowsAffected == 0)
            {
                throw new System.InvalidOperationException(
                    "Book not found.");
            }
        }

    
    
public async Task<List<Book>> SearchBooksByAuthorAsync(string author)
        {
            const string query = @"
        SELECT BookId, Title, Author, Category, Price, IsAvailable
        FROM Books
        WHERE Author LIKE @Author
        ORDER BY Title";

            var books = new List<Book>();

            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Author", $"%{author}%");

            await connection.OpenAsync();

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                books.Add(new Book(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetString(3),
                    reader.GetDecimal(4))
                {
                    IsAvailable = reader.GetBoolean(5)
                });
            }

            return books;
        }

    }
}

