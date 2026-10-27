
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using System;

namespace LibraryManagementSystem
{
    internal class Program
    {
        static void Main(string[] args)

        {
            DatabaseTester.TestConnectionAsync().GetAwaiter().GetResult();
            Console.ReadKey();
            LibraryService service = new LibraryService();

            while (true)
            {
                Console.WriteLine("\n========== LIBRARY MANAGEMENT SYSTEM ==========");
                Console.WriteLine("1. Display All Books");
                Console.WriteLine("2. Search Book by ID");
                Console.WriteLine("3. Search Book by Title");
                Console.WriteLine("4. Display All Members");
                Console.WriteLine("5. Register New Member");
                Console.WriteLine("6. Add New Book");
                Console.WriteLine("7. Borrow a Book");
                Console.WriteLine("8. Return a Book");
                Console.WriteLine("9. Display Borrowing Records");
                Console.WriteLine("10. Search Books by Author");
                Console.WriteLine("11. Exit");
                Console.WriteLine("12. Search Books by Category");
                Console.Write("Enter your choice: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Please enter a valid number!");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        service.DisplayBooks();
                        break;

                    case 2:
                        Console.Write("Enter Book ID: ");

                        if (int.TryParse(Console.ReadLine(), out int bookId))
                            service.SearchBookById(bookId);
                        else
                            Console.WriteLine("Invalid Book ID!");
                        break;

                    case 3:
                        Console.Write("Enter Book Title: ");
                        string title = Console.ReadLine() ?? "";
                        service.SearchBookByTitle(title);
                        break;

                    case 4:
                        service.DisplayMembers();
                        break;

                    case 5:
                        Console.Write("Enter Member ID: ");

                        if (!int.TryParse(Console.ReadLine(), out int memberId))
                        {
                            Console.WriteLine("Invalid Member ID!");
                            break;
                        }

                        Console.Write("Enter Member Name: ");
                        string name = Console.ReadLine() ?? "";

                        Console.Write("Enter Contact Number: ");
                        string contact = Console.ReadLine() ?? "";

                        Console.Write("Enter Email: ");
                        string email = Console.ReadLine() ?? "";

                        service.AddMember(
                            new Member(memberId, name, contact, email));
                        break;

                    case 6:
                        Console.Write("Enter Book ID: ");

                        if (!int.TryParse(Console.ReadLine(), out int newBookId))
                        {
                            Console.WriteLine("Invalid Book ID!");
                            break;
                        }


                        Console.Write("Enter Title: ");
                        string newTitle = Console.ReadLine() ?? "";

                        if (string.IsNullOrWhiteSpace(newTitle))
                        {
                            Console.WriteLine("Book title cannot be empty!");
                            break;
                        }



                        Console.Write("Enter Author: ");
                        string author = Console.ReadLine() ?? "";

                        if (string.IsNullOrWhiteSpace(author))
                        {
                            Console.WriteLine("Author name cannot be empty!");
                            break;
                        }


                        Console.Write("Enter Category: ");
                        string category = Console.ReadLine() ?? "";

                        Console.Write("Enter Price: ");

                        if (!decimal.TryParse(Console.ReadLine(), out decimal price)
                            || price < 0)
                        {
                            Console.WriteLine("Invalid price!");
                            break;
                        }

                        service.AddBook(
                            new Book(newBookId, newTitle, author, category, price));
                        break;

                    case 7:
                        Console.WriteLine("\n--- BORROW A BOOK ---");
                        service.DisplayBooks();

                        Console.Write("Enter Book ID: ");

                        if (!int.TryParse(Console.ReadLine(), out int borrowBookId))
                        {
                            Console.WriteLine("Invalid Book ID!");
                            break;
                        }

                        service.DisplayMembers();

                        Console.Write("Enter Member ID: ");

                        if (!int.TryParse(Console.ReadLine(), out int borrowMemberId))
                        {
                            Console.WriteLine("Invalid Member ID!");
                            break;
                        }

                        service.BorrowBook(borrowBookId, borrowMemberId);
                        break;

                    case 8:
                        Console.WriteLine("\n--- RETURN A BOOK ---");
                        service.DisplayBorrowRecords();

                        Console.Write("Enter Borrow ID: ");

                        if (!int.TryParse(Console.ReadLine(), out int borrowId))
                        {
                            Console.WriteLine("Invalid Borrow ID!");
                            break;
                        }

                        service.ReturnBook(borrowId);
                        break;

                    case 9:
                        Console.WriteLine("\n--- BORROWING RECORDS ---");
                        service.DisplayBorrowRecords();
                        break;

                    case 10:
                        Console.Write("Enter author name: ");
                        string author1 = Console.ReadLine() ?? "";
                        service.SearchBooksByAuthor(author1);
                        break;

                    case 11:
                        Console.WriteLine(
                            "Thank you for using the Library Management System!");
                        return;

                    case 12:
                        Console.Write("Enter book category: ");
                        string categorySearch = Console.ReadLine() ?? "";
                        service.SearchBooksByCategory(categorySearch);
                        break;

                    default:
                        Console.WriteLine("Invalid choice! Select 1 to 12.");
                        break;
                }
            }
        }
    }
}
