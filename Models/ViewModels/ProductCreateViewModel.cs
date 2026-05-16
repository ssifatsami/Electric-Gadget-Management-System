using Microsoft.AspNetCore.Mvc.Rendering;
using ElectricGadget.Web.Models.Entities;

namespace ElectricGadget.Web.Models.ViewModels
{
    public class ProductCreateViewModel
    {
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string Description { get; set; } = null!;
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
        public string? Model { get; set; }
        public string? Warranty { get; set; }
        public bool IsPublished { get; set; } = true;

        public IEnumerable<SelectListItem> Brands { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
    }
}
