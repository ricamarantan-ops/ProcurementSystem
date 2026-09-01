using System.ComponentModel.DataAnnotations;

namespace ProcurementSystem.Models
{
    public class Supplier
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string SupplierID { get; set; } = string.Empty; // e.g., SUP-001

        [Required]
        public string SupplierName { get; set; } = string.Empty; // Company Name column

        public string? ContactPerson { get; set; }

        public string? Phone { get; set; }

        [Required]
        public string Email { get; set; } = string.Empty;

        public string? Address { get; set; }

        public string? Category { get; set; } // Natural Materials, Finishing Materials, Textile & Fabric

        [Required]
        public string Status { get; set; } = "Active"; // Active, Inactive
    }
}