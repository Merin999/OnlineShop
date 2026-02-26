using OnlineShopping.Models;

namespace OnlineShopping.Services
{
    public interface ICartService
    {
        Task<Cart?> GetCartByCustomerIdAsync(int customerId);
        Task AddToCartAsync(int customerId, int productId);
        Task RemoveFromCartAsync(int customerId, int cartItemId);
        Task UpdateQuantityAsync(int customerId, int cartItemId, int quantity);
        Task ClearCartAsync(int customerId);
        Task<PurchaseSummary> GetPurchaseSummaryAsync(int customerId);
        Task<int> GetCartItemCountAsync(int customerId);
    }
}