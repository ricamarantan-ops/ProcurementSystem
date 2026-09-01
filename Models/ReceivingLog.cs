using System;
using System.ComponentModel.DataAnnotations;

namespace ProcurementSystem.Models
{
    public class ReceivingLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ReceivingNo { get; set; } = string.Empty; // e.g., REC-001

        public DateTime DateReceived { get; set; } = DateTime.Now;

        [Required]
        public string PORef { get; set; } = string.Empty; // PO Reference drop-down link

        [Required]
        public string ItemName { get; set; } = string.Empty;

        [Required]
        public string Supplier { get; set; } = string.Empty; // Supplier drop-down link

        public int QtyOrdered { get; set; }

        public int QtyReceived { get; set; }

        public string Unit { get; set; } = string.Empty; // e.g., Bundle, Yards

        public string Condition { get; set; } = "Good"; // Good, Damaged, Incomplete
    }
}