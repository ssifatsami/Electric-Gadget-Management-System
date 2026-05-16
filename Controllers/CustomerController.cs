using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ElectricGadget.Web.Data;
using ElectricGadget.Web.Models.Entities;

namespace ElectricGadget.Web.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string category, string search, string sort)
        {
            var productsQuery = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Reviews)
                .Where(p => p.IsPublished)
                .AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                productsQuery = productsQuery.Where(p => p.Category.Name == category);
            }

            if (!string.IsNullOrEmpty(search))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(search) || p.Brand.Name.Contains(search));
            }

            productsQuery = sort switch
            {
                "price_low" => productsQuery.OrderBy(p => p.Price),
                "price_high" => productsQuery.OrderByDescending(p => p.Price),
                "rating" => productsQuery.OrderByDescending(p => p.Reviews.Average(r => (double?)r.Rating) ?? 0),
                _ => productsQuery.OrderByDescending(p => p.Id)
            };

            var products = await productsQuery.ToListAsync();
            var categories = await _context.Categories.ToListAsync();

            ViewBag.Categories = categories;
            ViewBag.SelectedCategory = category;
            ViewBag.Search = search;
            ViewBag.Sort = sort;

            return View(products);
        }

        public async Task<IActionResult> Compare(int id1, int id2)
        {
            var p1 = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Features)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == id1);

            var p2 = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Features)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == id2);

            if (p1 == null || p2 == null) return NotFound();
            if (p1.CategoryId != p2.CategoryId)
            {
                TempData["Error"] = "You can only compare products from the same category.";
                return RedirectToAction("Index");
            }

            return View(new List<Product> { p1, p2 });
        }

        [HttpGet]
        public async Task<IActionResult> GetProductDetails(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Features)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == id);
            
            if (product == null) return NotFound();

            return Json(new {
                id = product.Id,
                name = product.Name,
                price = product.Price,
                brand = product.Brand.Name,
                category = product.Category.Name,
                imageUrl = product.ImageUrl,
                rating = product.AverageRating,
                stock = product.Stock,
                warranty = product.Warranty,
                model = product.Model
            });
        }
    }
}
