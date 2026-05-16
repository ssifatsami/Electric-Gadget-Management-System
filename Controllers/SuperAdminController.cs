using ElectricGadget.Web.Data;
using ElectricGadget.Web.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ElectricGadget.Web.Controllers
{
    public class SuperAdminController : Controller
    {
        private readonly ApplicationDbContext _db;

        public SuperAdminController(ApplicationDbContext db) => _db = db;

        // ── Guard: only Super Admin can access ──────────────────────────────────
        private bool IsSuperAdmin() =>
            HttpContext.Session.GetString("Role") == "Super Admin";

        private IActionResult Guard()
        {
            if (!IsSuperAdmin())
                return RedirectToAction("Login", "Account");
            return null!;
        }

        // ── Dashboard ───────────────────────────────────────────────────────────
        public async Task<IActionResult> Index()
        {
            var g = Guard(); if (g != null) return g;

            ViewBag.TotalGadgets    = await _db.Products.CountAsync();
            ViewBag.TotalUsers      = await _db.Users.CountAsync(u => u.Role == "Customer");
            ViewBag.TotalAdmins     = await _db.Users.CountAsync(u => u.Role == "Admin");
            ViewBag.TotalBranches   = await _db.Branches.CountAsync();
            ViewBag.PendingOrders   = await _db.Orders.CountAsync(o => o.Status == "Pending");
            ViewBag.TotalRevenue    = await _db.Orders
                                        .Where(o => o.Status == "Paid" || o.Status == "Shipped")
                                        .SumAsync(o => (decimal?)o.TotalAmount) ?? 0m;
            ViewBag.RecentLogs      = await _db.AuditLogs
                                        .OrderByDescending(l => l.Timestamp).Take(5).ToListAsync();
            ViewBag.Settings        = await _db.SystemSettings.FirstOrDefaultAsync();

            // --- Audit Log Chart Data (Last 7 Days) ---
            var last7Days = Enumerable.Range(0, 7)
                .Select(i => DateTime.Today.AddDays(-i))
                .Reverse()
                .ToList();

            var logCounts = await _db.AuditLogs
                .Where(l => l.Timestamp >= last7Days.First())
                .GroupBy(l => l.Timestamp.Date)
                .Select(group => new { Date = group.Key, Count = group.Count() })
                .ToListAsync();

            var chartLabels = last7Days.Select(d => d.ToString("dd MMM")).ToList();
            var chartData = last7Days.Select(d => logCounts.FirstOrDefault(lc => lc.Date == d)?.Count ?? 0).ToList();

            ViewBag.ChartLabels = chartLabels;
            ViewBag.ChartData = chartData;

            return View();
        }

        // ── Manage Admins ────────────────────────────────────────────────────────
        public async Task<IActionResult> ManageAdmins()
        {
            var g = Guard(); if (g != null) return g;
            var admins = await _db.Users
                .Where(u => u.Role == "Admin")
                .Include(u => u.Branch)
                .ToListAsync();
            return View(admins);
        }

        [HttpGet]
        public async Task<IActionResult> CreateAdmin()
        {
            var g = Guard(); if (g != null) return g;
            ViewBag.Branches = new SelectList(await _db.Branches.Where(b => b.IsActive).ToListAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAdmin(User model)
        {
            var g = Guard(); if (g != null) return g;
            if (await _db.Users.AnyAsync(u => u.UserID == model.UserID))
            {
                ModelState.AddModelError("UserID", "This Username already exists.");
            }
            if (ModelState.IsValid || !ModelState["UserID"]!.Errors.Any())
            {
                model.Role     = "Admin";
                model.IsActive = true;
                _db.Users.Add(model);
                await _db.SaveChangesAsync();
                await LogAsync("Create", "Admin", $"Created admin '{model.UserID}'");
                return RedirectToAction(nameof(ManageAdmins));
            }
            ViewBag.Branches = new SelectList(await _db.Branches.Where(b => b.IsActive).ToListAsync(), "Id", "Name");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditAdmin(string id)
        {
            var g = Guard(); if (g != null) return g;
            var user = await _db.Users.FindAsync(id);
            if (user == null || user.Role != "Admin") return NotFound();
            ViewBag.Branches = new SelectList(await _db.Branches.Where(b => b.IsActive).ToListAsync(), "Id", "Name", user.BranchId);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAdmin(User model)
        {
            var g = Guard(); if (g != null) return g;
            var user = await _db.Users.FindAsync(model.UserID);
            if (user == null) return NotFound();

            user.Name               = model.Name;
            user.Email              = model.Email;
            user.BranchId           = model.BranchId;
            user.CanAccessInventory = model.CanAccessInventory;
            user.CanAccessBilling   = model.CanAccessBilling;
            user.CanDownloadReports = model.CanDownloadReports;
            user.CanManageUsers     = model.CanManageUsers;
            if (!string.IsNullOrWhiteSpace(model.Password))
                user.Password = model.Password;

            await _db.SaveChangesAsync();
            await LogAsync("Update", "Admin", $"Updated admin '{user.UserID}'");
            return RedirectToAction(nameof(ManageAdmins));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAdmin(string id)
        {
            var g = Guard(); if (g != null) return g;
            var user = await _db.Users.FindAsync(id);
            if (user != null && user.Role == "Admin")
            {
                _db.Users.Remove(user);
                await _db.SaveChangesAsync();
                await LogAsync("Delete", "Admin", $"Deleted admin '{id}'");
            }
            return RedirectToAction(nameof(ManageAdmins));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleAdminStatus(string id)
        {
            var g = Guard(); if (g != null) return g;
            var user = await _db.Users.FindAsync(id);
            if (user != null && user.Role == "Admin")
            {
                user.IsActive = !user.IsActive;
                await _db.SaveChangesAsync();
                var action = user.IsActive ? "Activated" : "Suspended";
                await LogAsync(action, "Admin", $"{action} admin '{id}'");
            }
            return RedirectToAction(nameof(ManageAdmins));
        }

        // ── Permissions ──────────────────────────────────────────────────────────
        public async Task<IActionResult> Permissions()
        {
            var g = Guard(); if (g != null) return g;
            var admins = await _db.Users.Where(u => u.Role == "Admin").ToListAsync();
            return View(admins);
        }

        [HttpPost]
        public async Task<IActionResult> SavePermissions(string userId,
            bool canInventory, bool canBilling, bool canReports, bool canUsers)
        {
            var g = Guard(); if (g != null) return g;
            var user = await _db.Users.FindAsync(userId);
            if (user != null)
            {
                user.CanAccessInventory = canInventory;
                user.CanAccessBilling   = canBilling;
                user.CanDownloadReports = canReports;
                user.CanManageUsers     = canUsers;
                await _db.SaveChangesAsync();
                await LogAsync("Update", "Permissions", $"Updated permissions for '{userId}'");
            }
            return RedirectToAction(nameof(Permissions));
        }

        // ── Branches ─────────────────────────────────────────────────────────────
        public async Task<IActionResult> Branches()
        {
            var g = Guard(); if (g != null) return g;
            var branches = await _db.Branches.Include(b => b.Manager).ToListAsync();
            return View(branches);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBranch(string name, string location, string? managerId)
        {
            var g = Guard(); if (g != null) return g;
            _db.Branches.Add(new Branch
            {
                Name = name, Location = location, ManagerId = managerId, IsActive = true
            });
            await _db.SaveChangesAsync();
            await LogAsync("Create", "Branch", $"Created branch '{name}'");
            return RedirectToAction(nameof(Branches));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleBranch(int id)
        {
            var g = Guard(); if (g != null) return g;
            var branch = await _db.Branches.FindAsync(id);
            if (branch != null)
            {
                branch.IsActive = !branch.IsActive;
                await _db.SaveChangesAsync();
                await LogAsync("Toggle", "Branch", $"Toggled branch '{branch.Name}' to {(branch.IsActive?"Active":"Inactive")}");
            }
            return RedirectToAction(nameof(Branches));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            var g = Guard(); if (g != null) return g;
            var branch = await _db.Branches.FindAsync(id);
            if (branch != null)
            {
                _db.Branches.Remove(branch);
                await _db.SaveChangesAsync();
                await LogAsync("Delete", "Branch", $"Deleted branch '{branch.Name}'");
            }
            return RedirectToAction(nameof(Branches));
        }

        // ── System Settings ──────────────────────────────────────────────────────
        public async Task<IActionResult> Settings()
        {
            var g = Guard(); if (g != null) return g;
            var settings = await _db.SystemSettings.FirstOrDefaultAsync()
                           ?? new SystemSettings();
            return View(settings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Settings(SystemSettings model)
        {
            var g = Guard(); if (g != null) return g;
            var settings = await _db.SystemSettings.FirstOrDefaultAsync();
            if (settings == null) { _db.SystemSettings.Add(model); }
            else
            {
                settings.CompanyName  = model.CompanyName;
                settings.Currency     = model.Currency;
                settings.Timezone     = model.Timezone;
                settings.Language     = model.Language;
                settings.SupportEmail = model.SupportEmail;
                settings.Phone        = model.Phone;
                settings.LogoUrl      = model.LogoUrl;
                settings.UpdatedAt    = DateTime.Now;
            }
            await _db.SaveChangesAsync();
            await LogAsync("Update", "SystemSettings", "Updated system settings");
            TempData["Success"] = "Settings saved successfully!";
            return RedirectToAction(nameof(Settings));
        }

        // ── Audit Logs ───────────────────────────────────────────────────────────
        public async Task<IActionResult> AuditLogs(string? module, string? search)
        {
            var g = Guard(); if (g != null) return g;
            var query = _db.AuditLogs.AsQueryable();
            if (!string.IsNullOrEmpty(module))  query = query.Where(l => l.Module == module);
            if (!string.IsNullOrEmpty(search))  query = query.Where(l => l.Details.Contains(search) || l.UserName.Contains(search));
            
            var logs = await query.OrderByDescending(l => l.Timestamp).Take(200).ToListAsync();

            // --- Chart Data: Activity by Module ---
            var moduleStats = logs
                .GroupBy(l => l.Module)
                .Select(g => new { Module = g.Key, Count = g.Count() })
                .ToList();

            ViewBag.ModuleLabels = moduleStats.Select(s => s.Module).ToList();
            ViewBag.ModuleData   = moduleStats.Select(s => s.Count).ToList();

            ViewBag.Module = module;
            ViewBag.Search = search;
            return View(logs);
        }

        public async Task<IActionResult> ClearLogs()
        {
            var g = Guard(); if (g != null) return g;
            _db.AuditLogs.RemoveRange(_db.AuditLogs);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(AuditLogs));
        }

        // ── Manage All Users (Customers) ─────────────────────────────────────────
        public async Task<IActionResult> ManageUsers()
        {
            var g = Guard(); if (g != null) return g;
            var users = await _db.Users.Where(u => u.Role == "Customer").ToListAsync();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleUserStatus(string id)
        {
            var g = Guard(); if (g != null) return g;
            var user = await _db.Users.FindAsync(id);
            if (user != null)
            {
                user.IsActive = !user.IsActive;
                await _db.SaveChangesAsync();
                await LogAsync("Toggle", "User", $"Toggled user '{id}' IsActive={user.IsActive}");
            }
            return RedirectToAction(nameof(ManageUsers));
        }

        // ── Backup & Restore ────────────────────────────────────────────────────
        public IActionResult Backup()
        {
            var g = Guard(); if (g != null) return g;
            // Simulated backup list
            var backups = new List<dynamic>
            {
                new { Name = "db_backup_20240320.bak", Date = DateTime.Now.AddDays(-1), Size = "12 MB", Type = "Auto" },
                new { Name = "db_backup_20240315.bak", Date = DateTime.Now.AddDays(-6), Size = "11.5 MB", Type = "Manual" }
            };
            ViewBag.Backups = backups;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateBackup()
        {
            var g = Guard(); if (g != null) return g;
            
            try
            {
                // Fire and forget logging to prevent UI hang
                _ = LogAsync("Backup", "Database", "Manual system data download initiated");
                
                string content = $"-- Electric Gadget Management System Backup\n-- Generated: {DateTime.Now}\n-- Database: ElectricGadgetDB\n\nSELECT * FROM Products;\nSELECT * FROM Users;";
                var bytes = System.Text.Encoding.UTF8.GetBytes(content);
                return File(bytes, "application/octet-stream", $"system_backup_{DateTime.Now:yyyyMMdd_HHmm}.bak");
            }
            catch
            {
                return RedirectToAction(nameof(Backup));
            }
        }

        // ── Helper: write audit log ──────────────────────────────────────────────
        private async Task LogAsync(string action, string module, string details)
        {
            var userId   = HttpContext.Session.GetString("UserID") ?? "superadmin";
            var userName = HttpContext.Session.GetString("UserName") ?? "Super Admin";
            _db.AuditLogs.Add(new AuditLog
            {
                UserId    = userId,
                UserName  = userName,
                Action    = action,
                Module    = module,
                Details   = details,
                Timestamp = DateTime.Now,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            });
            await _db.SaveChangesAsync();
        }
    }
}
