using ElectricGadget.Web.Models.Entities;
using ElectricGadget.Web.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ElectricGadget.Web.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> FilterModelsAsync(decimal? minPrice, decimal? maxPrice, int? brandId, int? minRating);
        Task<IEnumerable<Product>> SearchSuggestionsAsync(string query);
        Task<IEnumerable<Product>> CompareModelsAsync(List<int> ids);
        Task<IEnumerable<Product>> GetTopSellingAsync(int count);
    }

    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        public ProductService(IProductRepository repository) => _repository = repository;

        public async Task<IEnumerable<Product>> FilterModelsAsync(decimal? minPrice, decimal? maxPrice, int? brandId, int? minRating)
        {
            var query = _repository.GetModelsQueryable();

            if (minPrice.HasValue) query = query.Where(m => m.Price >= minPrice.Value);
            if (maxPrice.HasValue) query = query.Where(m => m.Price <= maxPrice.Value);
            if (brandId.HasValue) query = query.Where(m => m.BrandId == brandId.Value);
            
            var results = await query.ToListAsync();

            if (minRating.HasValue)
            {
                results = results.Where(m => m.AverageRating >= (double)minRating.Value).ToList();
            }

            return results;
        }

        public async Task<IEnumerable<Product>> SearchSuggestionsAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return Enumerable.Empty<Product>();
            return await _repository.GetModelsQueryable()
                .Where(m => m.Name.Contains(query) || m.Brand.Name.Contains(query))
                .Take(5)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> CompareModelsAsync(List<int> ids)
        {
            var models = new List<Product>();
            foreach (var id in ids)
            {
                var model = await _repository.GetModelByIdAsync(id);
                if (model != null) models.Add(model);
            }
            return models;
        }

        public async Task<IEnumerable<Product>> GetTopSellingAsync(int count) => 
            await _repository.GetTopSellingModelsAsync(count);
    }
}
