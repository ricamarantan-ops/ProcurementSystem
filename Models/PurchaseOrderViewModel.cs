using System.Collections.Generic;

namespace ProcurementSystem.Models
{
    public class PurchaseOrderViewModel
    {
        // 📦 Holds the main grid table rows for existing entries
        public List<PurchaseOrder> ExistingOrders { get; set; } = new List<PurchaseOrder>();

        // 🔗 Holds the dynamic dropdown selection records from other tabs
        public List<PurchaseRequest> ApprovedRequests { get; set; } = new List<PurchaseRequest>();
        public List<Supplier> RegisteredSuppliers { get; set; } = new List<Supplier>();
    }
}