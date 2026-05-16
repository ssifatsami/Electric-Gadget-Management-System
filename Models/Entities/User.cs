using System;

namespace Electric_Gadget_Management.Models.Entities
{
    public class User
    {
        public string UserID { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // Super Admin, Admin, Customer/User
        public bool IsLocked { get; set; } = false;
        public int FailedAttempts { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
