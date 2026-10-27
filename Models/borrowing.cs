using System;

namespace LibraryManagementSystem.Models
{
    public class BorrowRecord
    {
        public int BorrowId { get; set; }
        public int BookId { get; set; }
        public int MemberId { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        public BorrowRecord(int borrowId, int bookId,
                            int memberId, DateTime borrowDate)
        {
            BorrowId = borrowId;
            BookId = bookId;
            MemberId = memberId;
            BorrowDate = borrowDate;
            ReturnDate = null;
        }
    }
}