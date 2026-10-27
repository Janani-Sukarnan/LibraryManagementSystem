
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
                    Console.WriteLine("No books found.");
                    return;
                }

                Console.WriteLine(
                    "\n{0,-10} {1,-25} {2,-20} {3,-18} {4,-10} {5,-15}",
                    "Book ID", "Title", "Author", "Category",
                    "Price", "Availability");

                Console.WriteLine(new string('-', 105));

                foreach (var book in books)
                {
                    Console.WriteLine(
                        "{0,-10} {1,-25} {2,-20} {3,-18} {4,-10:F2} {5,-15}",
                        book.BookId,
                        book.Title,
                        book.Author,
                        book.Category,
                        book.Price,
                        book.IsAvailable ? "Available" : "Borrowed");
                }
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
                    Console.WriteLine("No members found.");
                    return;
                }

                Console.WriteLine(
                    "\n{0,-12} {1,-25} {2,-20} {3,-30}",
                    "Member ID", "Name", "Contact Number", "Email");

                Console.WriteLine(new string('-', 90));

                foreach (var member in members)
                {
                    Console.WriteLine(
                        "{0,-12} {1,-25} {2,-20} {3,-30}",
                        member.MemberId,
                        member.Name,
                        member.ContactNumber,
                        member.Email);
                }
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
                    Console.WriteLine("No borrowing records found.");
                    return;
                }

                Console.WriteLine(
                    "\n{0,-12} {1,-10} {2,-12} {3,-22} {4,-22}",
                    "Borrow ID", "Book ID", "Member ID",
                    "Borrow Date", "Return Date");

                Console.WriteLine(new string('-', 80));

                foreach (var record in records)
                {
                    string returnDate = record.ReturnDate.HasValue
                        ? record.ReturnDate.Value.ToString("yyyy-MM-dd HH:mm")
                        : "Not returned";

                    Console.WriteLine(
                        "{0,-12} {1,-10} {2,-12} {3,-22} {4,-22}",
                        record.BorrowId,
                        record.BookId,
                        record.MemberId,
                        record.BorrowDate.ToString("yyyy-MM-dd HH:mm"),
                        returnDate);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Error displaying borrowing records: {ex.Message}");
            }
        }
    }
}
