using Microsoft.AspNetCore.Mvc;
using ElectricGadget.Web.Data;
using ElectricGadget.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ElectricGadget.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AccountController(ApplicationDbContext context) => _context = context;

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserID == username && u.Password == password);

            if (user != null)
            {
                if (!user.IsActive)
                {
                    ViewBag.Error = "Your account has been suspended. Contact Super Admin.";
                    return View();
                }

                // Store session info
                HttpContext.Session.SetString("UserID",   user.UserID);
                HttpContext.Session.SetString("UserName", user.Name);
                HttpContext.Session.SetString("Role",     user.Role);

                // Role-based redirect
                if (user.Role == "Super Admin")
                    return RedirectToAction("Index", "SuperAdmin");
                if (user.Role == "Admin")
                    return RedirectToAction("Index", "Admin");

                return RedirectToAction("Index", "Customer");
            }

            ViewBag.Error = "Invalid Username or Password.";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                user.Role     = "Customer";
                user.IsActive = true;
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login");
            }
            return View(user);
        }

        public IActionResult Dashboard() => View();
    }
}
