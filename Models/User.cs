using System.ComponentModel.DataAnnotations;

namespace ProcurementSystem.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty; // Full Name Column

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; } = "Staff"; // Admin, Procurement Officer, Staff

        [Required]
        public string Status { get; set; } = "Active"; // Active, Inactive
    }
}