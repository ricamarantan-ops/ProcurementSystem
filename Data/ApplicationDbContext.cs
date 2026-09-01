using Microsoft.EntityFrameworkCore;
using ProcurementSystem.Models;

namespace ProcurementSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Core security user identities table
        public DbSet<User> Users { get; set; }

        // Core procurement operation workflow tracking tables
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<PurchaseRequest> PurchaseRequests { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<ReceivingLog> ReceivingLogs { get; set; }
    }
}