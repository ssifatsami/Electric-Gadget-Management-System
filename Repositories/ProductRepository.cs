using ElectricGadget.Web.Data;
using ElectricGadget.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ElectricGadget.Web.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllModelsAsync();
        Task<Product?> GetModelByIdAsync(int id);
        IQueryable<Product> GetModelsQueryable();
        Task<IEnumerable<Product>> GetTopSellingModelsAsync(int count);
        Task AddReviewAsync(Review review);
    }

    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context) => _context = context;

        public async Task<IEnumerable<Product>> GetAllModelsAsync() => 
            await _context.Products.Include(m => m.Brand).Include(m => m.Reviews).ToListAsync();

        public async Task<Product?> GetModelByIdAsync(int id) => 
            await _context.Products.Include(m => m.Brand).Include(m => m.Features).Include(m => m.Reviews).FirstOrDefaultAsync(m => m.Id == id);

        public IQueryable<Product> GetModelsQueryable() => 
            _context.Products.Include(m => m.Brand).Include(m => m.Reviews).AsQueryable();

        public async Task<IEnumerable<Product>> GetTopSellingModelsAsync(int count)
        {
            return await _context.OrderItems
                .GroupBy(oi => oi.ProductId)
                .Select(g => new { Id = g.Key, Count = g.Sum(x => x.Quantity) })
                .OrderByDescending(x => x.Count)
                .Take(count)
                .Join(_context.Products.Include(m => m.Brand), x => x.Id, m => m.Id, (x, m) => m)
                .ToListAsync();
        }

        public async Task AddReviewAsync(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
        }
    }
}
