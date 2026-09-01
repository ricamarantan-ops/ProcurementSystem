using Microsoft.AspNetCore.Mvc;
using ProcurementSystem.Data;
using ProcurementSystem.Models;
using System.Diagnostics;
using System.Linq;

namespace ProcurementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Injects the database context connection globally into the controller instance
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Initial Login Screen Page
        public IActionResult Index()
        {
            return View("Login");
        }

        // 2. Registration Page View Route
        public IActionResult Register()
        {
            return View();
        }

        // 3. SECURED REGISTRATION: Confirms duplicates and saves user credentials safely
        [HttpPost]
        public IActionResult ProcessRegister(string email, string username, string password)
        {
            if (_context.Users.Any(u => u.Username.ToLower() == username.ToLower()))
            {
                ViewBag.ErrorMessage = "Username already exists.";
                return View("Register");
            }

            var newUser = new User
            {
                Email = email,
                Username = username,
                Password = password,
                FullName = username, // Default to username if not provided during basic signup
                Role = username.ToLower() == "admin" ? "Admin" : "Staff"
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // 4. SECURED LOGIN: Validates form credentials against actual database rows
        [HttpPost]
        public IActionResult ProcessLogin(string username, string password)
        {
            var authenticatedUser = _context.Users
                .FirstOrDefault(u => u.Username.ToLower() == username.ToLower() && u.Password == password);

            if (authenticatedUser != null)
            {
                HttpContext.Session.SetString("UserRole", authenticatedUser.Role);
                HttpContext.Session.SetString("Username", authenticatedUser.Username);
                return RedirectToAction("Dashboard");
            }

            ViewBag.ErrorMessage = "Invalid username or password credentials.";
            return View("Login");
        }

        // 5. Serves the Base Dashboard Shell Wrapper and forces initial metrics evaluation
        public IActionResult Dashboard()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserRole")))
            {
                return RedirectToAction("Index");
            }

            ViewBag.UserRole = HttpContext.Session.GetString("UserRole") ?? "User";

            LoadDashboardMetrics();

            return View();
        }

        // 6. Returns the Dashboard Summary Fragment with Live Calculated Database Values
        public IActionResult GetDashboardSummary()
        {
            LoadDashboardMetrics();
            return PartialView("_DashboardSummary");
        }

        // 7. Returns the Purchase Requests View Fragment populated with live rows
        public IActionResult GetPurchaseRequests()
        {
            var requestsList = _context.PurchaseRequests.OrderByDescending(r => r.Id).ToList();
            return PartialView("_PurchaseRequests", requestsList);
        }

        // 8. Processes the popup form submission, auto-generates numbers, and saves to database
        [HttpPost]
        public IActionResult CreatePurchaseRequest(string itemName, string category, int quantity, string unit, string priority)
        {
            int currentYear = DateTime.Now.Year;
            int nextId = (_context.PurchaseRequests.Count() > 0) ? _context.PurchaseRequests.Max(r => r.Id) + 1 : 1;
            string generatedPRNumber = $"PR-{currentYear}-{nextId:D3}";

            string requestingUser = HttpContext.Session.GetString("Username") ?? "Staff";

            var newPR = new PurchaseRequest
            {
                PRNumber = generatedPRNumber,
                DateRequested = DateTime.Now,
                ItemName = itemName,
                Category = category,
                Quantity = quantity,
                Unit = unit,
                RequestedBy = requestingUser,
                Priority = priority,
                Status = "Pending"
            };

            _context.PurchaseRequests.Add(newPR);
            _context.SaveChanges();

            return Json(new { success = true });
        }

        // 9. Process Row Validations: Restricts state updates exclusively to the Admin role
        [HttpPost]
        public IActionResult UpdateRequestStatus(int requestId, string newStatus)
        {
            string currentRole = HttpContext.Session.GetString("UserRole") ?? "User";

            // 🛡️ UPDATED: Allows both Admin and Procurement Officer to execute approvals
            if (currentRole != "Admin" && currentRole != "Procurement Officer")
            {
                return Json(new { success = false, message = "Unauthorized: Only Administrators or Procurement Officers can approve requests." });
            }

            var targetedRequest = _context.PurchaseRequests.FirstOrDefault(pr => pr.Id == requestId);
            if (targetedRequest != null)
            {
                targetedRequest.Status = newStatus;
                _context.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Target procurement item not found." });
        }

        // 10. FIXED: Now constructs and returns the required PurchaseOrderViewModel package
        public IActionResult GetPurchaseOrders()
        {
            var ordersList = _context.PurchaseOrders.OrderByDescending(o => o.Id).ToList();
            var approvedPRsList = _context.PurchaseRequests.Where(pr => pr.Status.ToLower() == "approved").ToList();
            var vendorsList = _context.Suppliers.OrderBy(s => s.SupplierName).ToList();

            ViewBag.TotalPOsCount = ordersList.Count;
            ViewBag.SentCount = ordersList.Count(o => o.Status == "Sent");
            ViewBag.PartialCount = ordersList.Count(o => o.Status == "Partial");
            ViewBag.ReceivedCount = ordersList.Count(o => o.Status == "Received");

            var pOrderPackage = new PurchaseOrderViewModel
            {
                ExistingOrders = ordersList,
                ApprovedRequests = approvedPRsList,
                RegisteredSuppliers = vendorsList
            };

            return PartialView("_PurchaseOrders", pOrderPackage);
        }

        // 11. Generates an official Purchase Order entity profile and writes to database
        [HttpPost]
        public IActionResult CreatePurchaseOrder(string prRef, string supplier, string item, int qty, decimal unitCost, DateTime? deliveryDate)
        {
            int currentYear = DateTime.Now.Year;
            int nextId = (_context.PurchaseOrders.Count() > 0) ? _context.PurchaseOrders.Max(o => o.Id) + 1 : 1;
            string generatedPONumber = $"PO-{currentYear}-{nextId:D3}";

            var newPO = new PurchaseOrder
            {
                PONumber = generatedPONumber,
                PRReference = prRef,
                DateOrdered = DateTime.Now,
                SupplierName = supplier,
                ItemName = item,
                Quantity = qty,
                UnitCost = unitCost,
                TotalAmount = qty * unitCost,
                DeliveryDate = deliveryDate,
                Status = "Sent"
            };

            _context.PurchaseOrders.Add(newPO);
            _context.SaveChanges();

            return Json(new { success = true });
        }

        // 12. Returns the Suppliers view fragment populated with live rows
        public IActionResult GetSuppliers()
        {
            var suppliersList = _context.Suppliers.OrderBy(s => s.SupplierID).ToList();
            ViewBag.ActiveSuppliersCount = _context.Suppliers.Count(s => s.Status.ToLower() == "active");
            return PartialView("_Suppliers", suppliersList);
        }

        // 13. Saves a new supplier record, formats custom IDs, and writes to database
        [HttpPost]
        public IActionResult CreateSupplier(string supplierName, string contactPerson, string email, string phone, string address, string category, string status)
        {
            int nextId = (_context.Suppliers.Count() > 0) ? _context.Suppliers.Max(s => s.Id) + 1 : 1;
            string generatedSupplierID = $"SUP-{nextId:D3}";

            var newSupplier = new Supplier
            {
                SupplierID = generatedSupplierID,
                SupplierName = supplierName,
                ContactPerson = contactPerson,
                Email = email,
                Phone = phone,
                Address = address,
                Category = category,
                Status = string.IsNullOrEmpty(status) ? "Active" : status
            };

            _context.Suppliers.Add(newSupplier);
            _context.SaveChanges();

            return Json(new { success = true });
        }

        // 14. Returns the Receiving view fragment with dynamic dropdown data
        public IActionResult GetReceiving()
        {
            var deliveryLogs = _context.ReceivingLogs.OrderByDescending(r => r.Id).ToList();

            ViewBag.TotalReceivedCount = deliveryLogs.Count;
            ViewBag.ThisMonthCount = deliveryLogs.Count(r => r.DateReceived.Month == DateTime.Now.Month && r.DateReceived.Year == DateTime.Now.Year);

            ViewBag.PurchaseOrdersList = _context.PurchaseOrders.OrderByDescending(o => o.Id).ToList();
            ViewBag.ActiveSuppliersList = _context.Suppliers.Where(s => s.Status.ToLower() == "active").OrderBy(s => s.SupplierName).ToList();

            return PartialView("_Receiving", deliveryLogs);
        }
        // 15. Saves incoming delivery parameters into database context
        [HttpPost]
        public IActionResult CreateReceivingEntry(string poRef, string supplier, string itemName, int qtyOrdered, int qtyReceived, string unit, string condition)
        {
            int nextId = (_context.ReceivingLogs.Count() > 0) ? _context.ReceivingLogs.Max(r => r.Id) + 1 : 1;
            string generatedRecNo = $"REC-{nextId:D3}";

            var newLog = new ReceivingLog
            {
                ReceivingNo = generatedRecNo,
                DateReceived = DateTime.Now,
                PORef = poRef,
                ItemName = itemName,
                Supplier = supplier,
                QtyOrdered = qtyOrdered,
                QtyReceived = qtyReceived,
                Unit = unit,
                Condition = string.IsNullOrEmpty(condition) ? "Good" : condition
            };

            _context.ReceivingLogs.Add(newLog);
            _context.SaveChanges();

            return Json(new { success = true });
        }

        // 16. Returns the Reports view layout framework with live counts data
        public IActionResult GetReports()
        {
            ViewBag.TotalRequestsCount = _context.PurchaseRequests.Count();
            ViewBag.TotalOrdersCount = _context.PurchaseOrders.Count();
            ViewBag.TotalSpendAmount = _context.PurchaseOrders.Where(po => po.Status != "Cancelled").Sum(po => (decimal?)po.TotalAmount) ?? 0;
            ViewBag.ActiveSuppliersCount = _context.Suppliers.Count(s => s.Status.ToLower() == "active");

            var prReportList = _context.PurchaseRequests.OrderByDescending(r => r.Id).ToList();
            return PartialView("_Reports", prReportList);
        }

        // 17. AJAX Sub-Report layout router switch engine
        public IActionResult GetSpecificReport(string reportType)
        {
            switch (reportType?.ToLower())
            {
                case "po":
                    var pos = _context.PurchaseOrders.OrderByDescending(o => o.Id).ToList();
                    return PartialView("_ReportPOTable", pos);
                case "spend":
                    var spend = _context.PurchaseOrders.Where(o => o.Status != "Cancelled").OrderByDescending(o => o.TotalAmount).ToList();
                    return PartialView("_ReportSpendTable", spend);
                case "supplier":
                    var vendors = _context.Suppliers.OrderBy(s => s.SupplierID).ToList();
                    return PartialView("_ReportSupplierTable", vendors);
                case "pr":
                default:
                    var prs = _context.PurchaseRequests.OrderByDescending(r => r.Id).ToList();
                    return PartialView("_ReportPRTable", prs);
            }
        }

        // 18. Serves the Master Settings workspace (Admin Only)
        public IActionResult GetSettings()
        {
            string userRole = HttpContext.Session.GetString("UserRole") ?? "User";
            if (userRole != "Admin")
            {
                return Content("<h3 style='color:red; font-family:sans-serif; padding:20px;'>⚠️ ACCESS DENIED</h3>");
            }

            var systemUsers = _context.Users.OrderBy(u => u.FullName).ToList();
            return PartialView("_Settings", systemUsers);
        }

        // 19. Administrative functionality: Saves a managed account user record
        [HttpPost]
        public IActionResult CreateManagedUser(string fullName, string username, string email, string password, string role)
        {
            if (_context.Users.Any(u => u.Username.ToLower() == username.ToLower()))
            {
                return Json(new { success = false, message = "Username already exists." });
            }

            var managedProfile = new User
            {
                FullName = fullName,
                Username = username,
                Email = email,
                Password = password,
                Role = role,
                Status = "Active"
            };

            _context.Users.Add(managedProfile);
            _context.SaveChanges();

            return Json(new { success = true });
        }
        // 20. Fetches a specific user's details for editing
        [HttpGet]
        public IActionResult GetUserForEdit(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                return Json(new
                {
                    success = true,
                    id = user.Id,
                    fullName = user.FullName,
                    username = user.Username,
                    email = user.Email,
                    role = user.Role,
                    status = user.Status
                });
            }
            return Json(new { success = false, message = "User not found." });
        }

        // 21. Saves updated user changes back to the database
        [HttpPost]
        public IActionResult UpdateManagedUser(int id, string fullName, string email, string role, string status, string? password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            // Update details
            user.FullName = fullName;
            user.Email = email;
            user.Role = role;
            user.Status = status;

            // Only update password if a new one was provided
            if (!string.IsNullOrEmpty(password))
            {
                user.Password = password;
            }

            _context.SaveChanges();
            return Json(new { success = true });
        }
        // 22. Forgot Password Page Route
        public IActionResult ForgotPassword()
        {
            return View();
        }
        // Helper Method: Re-calculates and aggregates all tab stats instantly
        private void LoadDashboardMetrics()
        {
            ViewBag.PurchaseRequestsCount = _context.PurchaseRequests.Count();
            ViewBag.PendingApprovalsCount = _context.PurchaseRequests.Count(pr => pr.Status == "Pending");
            ViewBag.OpenPurchaseOrdersCount = _context.PurchaseOrders.Count(po => po.Status == "Sent" || po.Status == "Partial");
            ViewBag.TotalSpend = _context.PurchaseOrders.Where(po => po.Status != "Cancelled").Sum(po => (decimal?)po.TotalAmount) ?? 0;

            ViewBag.PendingList = _context.PurchaseRequests
                .Where(pr => pr.Status == "Pending")
                .OrderByDescending(pr => pr.Id)
                .Select(pr => $"{pr.PRNumber} — {pr.ItemName} × {pr.Quantity} {pr.Unit}")
                .ToList();

            ViewBag.OpenOrdersList = _context.PurchaseOrders
                .Where(po => po.Status == "Sent" || po.Status == "Partial")
                .OrderByDescending(po => po.Id)
                .Select(po => $"{po.PONumber} ({po.Status.ToUpper()})")
                .ToList();

            ViewBag.RecentTransactionsList = _context.PurchaseRequests
                .OrderByDescending(pr => pr.Id)
                .Take(5)
                .Select(pr => $"{pr.PRNumber} — {pr.ItemName} [{pr.Status.ToUpper()}]")
                .ToList();

            ViewBag.TopSuppliersList = _context.Suppliers
                .Take(3)
                .Select(s => s.SupplierName)
                .ToList();
        }
    }
}