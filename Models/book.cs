using System;

namespace LibraryManagementSystem.Models
{
    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }

        public Book(int bookId, string title, string author,
                    string category, decimal price)
        {
            BookId = bookId;
            Title = title;
            Author = author;
            Category = category;
            Price = price;
            IsAvailable = true;
        }
    }
}