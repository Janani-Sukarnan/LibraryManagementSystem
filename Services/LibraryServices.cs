
using System;
using System.Linq;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repository;

namespace LibraryManagementSystem.Services
{
    public class LibraryService
    {
        private readonly BookRepository _bookRepository;
        private readonly MemberRepository _memberRepository;
        private readonly BorrowRecordRepository _borrowRecordRepository;

        public LibraryService()
        {
            _bookRepository = new BookRepository();
            _memberRepository = new MemberRepository();
            _borrowRecordRepository = new BorrowRecordRepository();
        }

        // Display all books

        public void DisplayBooks()
        {
            try
            {
                var books = _bookRepository
                    .GetAllBooksAsync()
                    .GetAwaiter()
                    .GetResult();

                if (books.Count == 0)
                {
                    Console.WriteLine("\nNo books found in the library.");
                    return;
                }

                Console.WriteLine("\n========== BOOK CATALOGUE ==========\n");

                string separator = new string('-', 115);

                Console.WriteLine(separator);

                Console.WriteLine(
                    $"{"Book ID",-10} {"Title",-25} {"Author",-20} {"Category",-18} {"Price",12} {"Status",-15}");

                Console.WriteLine(separator);

                foreach (var book in books)
                {
                    string title = book.Title.Length > 25
                        ? book.Title[..22] + "..."
                        : book.Title;

                    string author = book.Author.Length > 20
                        ? book.Author[..17] + "..."
                        : book.Author;

                    string category = book.Category.Length > 18
                        ? book.Category[..15] + "..."
                        : book.Category;

                    string status = book.IsAvailable
                        ? "Available"
                        : "Borrowed";

                    Console.WriteLine(
                        $"{book.BookId,-10} {title,-25} {author,-20} {category,-18} {book.Price,12:F2} {status,-15}");
                }

                Console.WriteLine(separator);
                Console.WriteLine($"Total books: {books.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error displaying books: {ex.Message}");
            }
        }



        // Search a book by ID
        public void SearchBookById(int bookId)
        {
            try
            {
                var book = _bookRepository
                    .GetBookByIdAsync(bookId)
                    .GetAwaiter()
                    .GetResult();

                if (book == null)
                {
                    Console.WriteLine("Book not found.");
                    return;
                }

                Console.WriteLine("\n--- BOOK DETAILS ---");
                Console.WriteLine($"Book ID: {book.BookId}");
                Console.WriteLine($"Title: {book.Title}");
                Console.WriteLine($"Author: {book.Author}");
                Console.WriteLine($"Category: {book.Category}");
                Console.WriteLine($"Price: {book.Price:F2}");
                Console.WriteLine(
                    $"Availability: {(book.IsAvailable ? "Available" : "Borrowed")}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching for book: {ex.Message}");
            }
        }

        // Search a book by title
        public void SearchBookByTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Please enter a book title.");
                return;
            }

            try
            {
                var books = _bookRepository
                    .GetAllBooksAsync()
                    .GetAwaiter()
                    .GetResult();

                var matchingBooks = books
                    .Where(book => book.Title.Contains(
                        title,
                        StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (matchingBooks.Count == 0)
                {
                    Console.WriteLine("No books found with that title.");
                    return;
                }

                Console.WriteLine("\n--- SEARCH RESULTS ---");

                foreach (var book in matchingBooks)
                {
                    Console.WriteLine($"Book ID: {book.BookId}");
                    Console.WriteLine($"Title: {book.Title}");
                    Console.WriteLine($"Author: {book.Author}");
                    Console.WriteLine($"Category: {book.Category}");
                    Console.WriteLine($"Price: {book.Price:F2}");
                    Console.WriteLine(
                        $"Availability: {(book.IsAvailable ? "Available" : "Borrowed")}");
                    Console.WriteLine(new string('-', 35));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching for book: {ex.Message}");
            }
        }

        // Display all members

        public void DisplayMembers()
        {
            try
            {
                var members = _memberRepository
                    .GetAllMembersAsync()
                    .GetAwaiter()
                    .GetResult();

                if (members.Count == 0)
                {
                    Console.WriteLine("\nNo registered members found.");
                    return;
                }

                Console.WriteLine("\n========== REGISTERED MEMBERS ==========\n");

                Console.WriteLine(
                    $"{"Member ID",-12} {"Name",-25} {"Contact Number",-20} {"Email",-35}");

                Console.WriteLine(new string('-', 95));

                foreach (var member in members)
                {
                    string name = member.Name.Length > 25
                        ? member.Name[..22] + "..."
                        : member.Name;

                    string contact = member.ContactNumber.Length > 20
                        ? member.ContactNumber[..17] + "..."
                        : member.ContactNumber;

                    string email = member.Email.Length > 35
                        ? member.Email[..32] + "..."
                        : member.Email;

                    Console.WriteLine(
                        $"{member.MemberId,-12} {name,-25} {contact,-20} {email,-35}");
                }

                Console.WriteLine(new string('-', 95));
                Console.WriteLine($"Total registered members: {members.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error displaying members: {ex.Message}");
            }
        }


        // Register a new member
        public void AddMember(Member member)
        {
            if (member == null)
            {
                Console.WriteLine("Invalid member details.");
                return;
            }

            if (string.IsNullOrWhiteSpace(member.Name) ||
                string.IsNullOrWhiteSpace(member.ContactNumber) ||
                string.IsNullOrWhiteSpace(member.Email))
            {
                Console.WriteLine("All member details are required.");
                return;
            }

            try
            {
                _memberRepository
                    .AddMemberAsync(member)
                    .GetAwaiter()
                    .GetResult();

                Console.WriteLine("Member registered successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registering member: {ex.Message}");
            }
        }

        // Add a new book
        public void AddBook(Book book)
        {
            if (book == null)
            {
                Console.WriteLine("Invalid book details.");
                return;
            }

            if (string.IsNullOrWhiteSpace(book.Title) ||
                string.IsNullOrWhiteSpace(book.Author) ||
                string.IsNullOrWhiteSpace(book.Category) ||
                book.Price < 0)
            {
                Console.WriteLine("Please enter valid book details.");
                return;
            }

            try
            {
                _bookRepository
                    .AddBookAsync(book)
                    .GetAwaiter()
                    .GetResult();

                Console.WriteLine("Book added successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding book: {ex.Message}");
            }
        }

        // Borrow a book
        public void BorrowBook(int bookId, int memberId)
        {
            try
            {
                int borrowId = _borrowRecordRepository
                    .BorrowBookAsync(bookId, memberId)
                    .GetAwaiter()
                    .GetResult();

                Console.WriteLine("Book borrowed successfully!");
                Console.WriteLine($"Borrow ID: {borrowId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error borrowing book: {ex.Message}");
            }
        }

        // Return a borrowed book
        public void ReturnBook(int borrowId)
        {
            try
            {
                _borrowRecordRepository
                    .ReturnBookAsync(borrowId)
                    .GetAwaiter()
                    .GetResult();

                Console.WriteLine("Book returned successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error returning book: {ex.Message}");
            }
        }

        // Display all borrowing records

        public void DisplayBorrowRecords()
        {
            try
            {
                var records = _borrowRecordRepository
                    .GetAllBorrowRecordsAsync()
                    .GetAwaiter()
                    .GetResult();

                if (records.Count == 0)
                {
                    Console.WriteLine("\nNo borrowing records found.");
                    return;
                }

                Console.WriteLine("\n========== BORROWING HISTORY ==========\n");

                Console.WriteLine(
                    $"{"Borrow ID",-12} {"Book ID",-10} {"Member ID",-12} {"Borrow Date",-20} {"Return Date",-20} {"Status",-12}");

                Console.WriteLine(new string('-', 100));

                foreach (var record in records)
                {
                    string borrowDate =
                        record.BorrowDate.ToString("yyyy-MM-dd HH:mm");

                    string returnDate = record.ReturnDate.HasValue
                        ? record.ReturnDate.Value.ToString("yyyy-MM-dd HH:mm")
                        : "Not returned";

                    string status = record.ReturnDate.HasValue
                        ? "Returned"
                        : "Borrowed";

                    Console.WriteLine(
                        $"{record.BorrowId,-12} {record.BookId,-10} {record.MemberId,-12} {borrowDate,-20} {returnDate,-20} {status,-12}");
                }

                Console.WriteLine(new string('-', 100));
                Console.WriteLine($"Total borrowing records: {records.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Error displaying borrowing records: {ex.Message}");
            }
        }

