using System;

namespace LibraryManagementSystem.Models
{
    public class Member
    {
        public int MemberId { get; set; }
        public string Name { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }

        public Member(int memberId, string name,
                      string contactNumber, string email)
        {
            MemberId = memberId;
            Name = name;
            ContactNumber = contactNumber;
            Email = email;
        }
    }
}