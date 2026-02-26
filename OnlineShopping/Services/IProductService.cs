using OnlineShopping.Models;

namespace OnlineShopping.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category);
        Task<Product?> GetProductByIdAsync(int id);
        Task<IEnumerable<string>> GetCategoriesAsync();
    }
}