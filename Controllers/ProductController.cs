using ElectricGadget.Web.Models.Entities;
using ElectricGadget.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElectricGadget.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService) => _productService = productService;

        public async Task<IActionResult> Index(decimal? minPrice, decimal? maxPrice, int? brandId, int? minRating)
        {
            var models = await _productService.FilterModelsAsync(minPrice, maxPrice, brandId, minRating);
            return View(models);
        }

        public async Task<IActionResult> Details(int id)
        {
            return View(); 
        }

        [HttpGet]
        public async Task<JsonResult> SearchSuggestions(string q)
        {
            var suggestions = await _productService.SearchSuggestionsAsync(q);
            return Json(suggestions.Select(s => new { id = s.Id, name = s.Name, brand = s.Brand.Name }));
        }

        public async Task<IActionResult> Compare(string ids)
        {
            if (string.IsNullOrEmpty(ids)) return RedirectToAction("Index");
            var idList = ids.Split(',').Select(int.Parse).ToList();
            var models = await _productService.CompareModelsAsync(idList);
            return View(models);
        }

        public async Task<IActionResult> TopSelling()
        {
            var topProducts = await _productService.GetTopSellingAsync(10);
            return View(topProducts);
        }
    }
}
