using ElectricGadget.Web.Data;
using ElectricGadget.Web.Models.Entities;
using ElectricGadget.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ElectricGadget.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.Include(p => p.Brand).ToListAsync();
            return View(products);
        }

        // --- Super Admin / Admin Features ---

        public async Task<IActionResult> ManageUsers()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ManageUsers));
        }

        public async Task<IActionResult> Earnings()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();
            return View(orders);
        }

        // --- Product Management ---

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var viewModel = new ProductCreateViewModel
            {
                Brands = await _context.Brands.Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.Name
                }).ToListAsync(),
                Categories = await _context.Categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToListAsync()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateViewModel productVM)
        {
            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    Name = productVM.Name,
                    Price = productVM.Price,
                    Description = productVM.Description,
                    Stock = productVM.Stock,
                    ImageUrl = string.IsNullOrEmpty(productVM.ImageUrl) ? "/images/default.jpg" : productVM.ImageUrl,
                    BrandId = productVM.BrandId,
                    CategoryId = productVM.CategoryId,
                    Model = productVM.Model,
                    Warranty = productVM.Warranty,
                    IsPublished = productVM.IsPublished
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                await LogAsync("Create", "Product", $"Created product '{product.Name}' (ID: {product.Id})");
                return RedirectToAction(nameof(Index));
            }

            productVM.Brands = await _context.Brands.Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.Name
            }).ToListAsync();
            productVM.Categories = await _context.Categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToListAsync();

            return View(productVM);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            var viewModel = new ProductEditViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                Stock = product.Stock,
                ImageUrl = product.ImageUrl,
                BrandId = product.BrandId,
                CategoryId = product.CategoryId ?? 0,
                Model = product.Model,
                Warranty = product.Warranty,
                IsPublished = product.IsPublished,
                Brands = await _context.Brands.Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.Name }).ToListAsync(),
                Categories = await _context.Categories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToListAsync()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductEditViewModel productVM)
        {
            if (ModelState.IsValid)
            {
                var product = await _context.Products.FindAsync(productVM.Id);
                if (product == null) return NotFound();

                product.Name = productVM.Name;
                product.Price = productVM.Price;
                product.Description = productVM.Description;
                product.Stock = productVM.Stock;
                product.ImageUrl = string.IsNullOrEmpty(productVM.ImageUrl) ? "/images/default.jpg" : productVM.ImageUrl;
                product.BrandId = productVM.BrandId;
                product.CategoryId = productVM.CategoryId;
                product.Model = productVM.Model;
                product.Warranty = productVM.Warranty;
                product.IsPublished = productVM.IsPublished;

                _context.Update(product);
                await _context.SaveChangesAsync();
                await LogAsync("Update", "Product", $"Updated product '{product.Name}' (ID: {product.Id})");
                return RedirectToAction(nameof(Index));
            }

            productVM.Brands = await _context.Brands.Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.Name }).ToListAsync();
            productVM.Categories = await _context.Categories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToListAsync();
            return View(productVM);
        }

        private async Task LogAsync(string action, string module, string details)
        {
            var userId   = HttpContext.Session.GetString("UserID") ?? "admin";
            var userName = HttpContext.Session.GetString("UserName") ?? "Admin User";
            _context.AuditLogs.Add(new AuditLog
            {
                UserId    = userId,
                UserName  = userName,
                Action    = action,
                Module    = module,
                Details   = details,
                Timestamp = DateTime.Now,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            });
            await _context.SaveChangesAsync();
        }
    }
}
