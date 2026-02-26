using Microsoft.EntityFrameworkCore;
using OnlineShopping.Data;
using OnlineShopping.Models;

namespace OnlineShopping.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductService> _logger;

        public ProductService(
            ApplicationDbContext context,
            ILogger<ProductService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Get all active products
        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            try
            {
                return await _context.Products
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.Category)
                    .ThenBy(p => p.ProductName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all products");
                throw;
            }
        }

        // Get products by category
        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(
            string category)
        {
            try
            {
                return await _context.Products
                    .Where(p => p.IsActive && p.Category == category)
                    .OrderBy(p => p.ProductName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error fetching products for category {Category}",
                    category);
                throw;
            }
        }

        // Get single product by Id
        public async Task<Product?> GetProductByIdAsync(int id)
        {
            try
            {
                return await _context.Products
                    .FirstOrDefaultAsync(p => p.ProductId == id && p.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error fetching product {ProductId}", id);
                throw;
            }
        }

        // Get all distinct categories
        public async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            try
            {
                return await _context.Products
                    .Where(p => p.IsActive)
                    .Select(p => p.Category)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching categories");
                throw;
            }
        }
    }
}