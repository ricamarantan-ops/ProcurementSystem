using System;
using System.ComponentModel.DataAnnotations;

namespace ProcurementSystem.Models
{
    public class PurchaseRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string PRNumber { get; set; } = string.Empty;

        public DateTime DateRequested { get; set; } = DateTime.Now;

        [Required]
        public string ItemName { get; set; } = string.Empty;

        public string? Category { get; set; }

        public int Quantity { get; set; }

        public string? Unit { get; set; }

        public string? RequestedBy { get; set; }

        public string? Priority { get; set; }

        public string Status { get; set; } = "Pending";
    }
}