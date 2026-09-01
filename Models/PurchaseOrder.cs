using System;
using System.ComponentModel.DataAnnotations;

namespace ProcurementSystem.Models
{
    public class PurchaseOrder
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string PONumber { get; set; } = string.Empty; // e.g., PO-2026-001

        [Required]
        public string PRReference { get; set; } = string.Empty; // PR Ref Column

        public DateTime DateOrdered { get; set; } = DateTime.Now;

        [Required]
        public string SupplierName { get; set; } = string.Empty;

        [Required]
        public string ItemName { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal UnitCost { get; set; } // Unit Cost (P) Column

        public decimal TotalAmount { get; set; } // Total (P) Column

        public DateTime? DeliveryDate { get; set; } // Expected Arrival

        [Required]
        public string Status { get; set; } = "Sent"; // Sent, Partial, Received
    }
}