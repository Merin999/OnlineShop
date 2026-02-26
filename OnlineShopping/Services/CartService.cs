using Microsoft.EntityFrameworkCore;
using OnlineShopping.Data;
using OnlineShopping.Models;

namespace OnlineShopping.Services
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CartService> _logger;

        // Read discount settings from appsettings.json
        private readonly decimal _minimumAmountForDiscount;
        private readonly decimal _cartDiscountPercentage;

        public CartService(
            ApplicationDbContext context,
            IConfiguration configuration,
            ILogger<CartService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;

            _minimumAmountForDiscount = _configuration
                .GetValue<decimal>(
                    "DiscountSettings:MinimumCartAmountForDiscount", 5000);

            _cartDiscountPercentage = _configuration
                .GetValue<decimal>(
                    "DiscountSettings:DiscountPercentage", 10);
        }

        // ── Get Cart ─────────────────────────────────────────
        public async Task<Cart?> GetCartByCustomerIdAsync(int customerId)
        {
            try
            {
                return await _context.Carts
                    .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Product)
                    .FirstOrDefaultAsync(c => c.CustomerId == customerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error fetching cart for customer {CustomerId}",
                    customerId);
                throw;
            }
        }

        // ── Add To Cart ──────────────────────────────────────
        public async Task AddToCartAsync(int customerId, int productId)
        {
            try
            {
                // Check product exists
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.ProductId == productId && p.IsActive);

                if (product == null)
                    throw new Exception("Product not found");

                // Check stock
                if (product.StockQuantity <= 0)
                    throw new Exception($"'{product.ProductName}' is out of stock");

                // Get or create cart for this customer
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.CustomerId == customerId);

                if (cart == null)
                {
                    // Create new cart if customer doesn't have one
                    cart = new Cart
                    {
                        CustomerId = customerId,
                        CreatedBy = "System",
                        CreatedDate = DateTime.UtcNow
                    };
                    _context.Carts.Add(cart);
                    await _context.SaveChangesAsync();
                }

                // Check if product already in cart
                var existingItem = cart.CartItems
                    .FirstOrDefault(ci => ci.ProductId == productId);

                if (existingItem != null)
                {
                    // Increase quantity if already exists
                    existingItem.Quantity++;
                    existingItem.UpdatedDate = DateTime.UtcNow;
                    existingItem.UpdatedBy = "System";
                }
                else
                {
                    // Add new item — apply product discount immediately
                    var cartItem = new CartItem
                    {
                        CartId = cart.Id,
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        OriginalPrice = product.Price,
                        DiscountPercentage = product.DiscountPercentage,
                        UnitPrice = product.DiscountedPrice, // ← Discount applied here
                        Quantity = 1,
                        CreatedBy = "System",
                        CreatedDate = DateTime.UtcNow
                    };
                    cart.CartItems.Add(cartItem);
                }

                // Update cart timestamp
                cart.UpdatedDate = DateTime.UtcNow;
                cart.UpdatedBy = "System";

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error adding product {ProductId} to cart for customer {CustomerId}",
                    productId, customerId);
                throw;
            }
        }

        // ── Remove From Cart ─────────────────────────────────
        public async Task RemoveFromCartAsync(int customerId, int cartItemId)
        {
            try
            {
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.CustomerId == customerId);

                if (cart == null)
                    throw new Exception("Cart not found");

                var item = cart.CartItems
                    .FirstOrDefault(ci => ci.Id == cartItemId);

                if (item == null)
                    throw new Exception("Cart item not found");

                _context.CartItems.Remove(item);

                cart.UpdatedDate = DateTime.UtcNow;
                cart.UpdatedBy = "System";

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error removing cart item {CartItemId} for customer {CustomerId}",
                    cartItemId, customerId);
                throw;
            }
        }

        // ── Update Quantity ──────────────────────────────────
        public async Task UpdateQuantityAsync(
            int customerId, int cartItemId, int quantity)
        {
            try
            {
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.CustomerId == customerId);

                if (cart == null)
                    throw new Exception("Cart not found");

                var item = cart.CartItems
                    .FirstOrDefault(ci => ci.Id == cartItemId);

                if (item == null)
                    throw new Exception("Cart item not found");

                if (quantity < 1)
                {
                    // Remove item if quantity is 0
                    _context.CartItems.Remove(item);
                }
                else
                {
                    item.Quantity = quantity;
                    item.UpdatedDate = DateTime.UtcNow;
                    item.UpdatedBy = "System";
                }

                cart.UpdatedDate = DateTime.UtcNow;
                cart.UpdatedBy = "System";

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error updating quantity for cart item {CartItemId}",
                    cartItemId);
                throw;
            }
        }

        // ── Clear Cart ───────────────────────────────────────
        public async Task ClearCartAsync(int customerId)
        {
            try
            {
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.CustomerId == customerId);

                if (cart == null) return;

                _context.CartItems.RemoveRange(cart.CartItems);

                cart.UpdatedDate = DateTime.UtcNow;
                cart.UpdatedBy = "System";

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error clearing cart for customer {CustomerId}",
                    customerId);
                throw;
            }
        }

        // ── Get Cart Item Count ──────────────────────────────
        public async Task<int> GetCartItemCountAsync(int customerId)
        {
            try
            {
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.CustomerId == customerId);

                return cart?.CartItems.Sum(ci => ci.Quantity) ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error getting cart count for customer {CustomerId}",
                    customerId);
                return 0;
            }
        }

        // ── Purchase Summary with Discount Logic ─────────────
        /// <summary>
        /// Core Discount Logic:
        /// 1. Product discount  → applied when item is added to cart
        /// 2. Cart discount 10% → applied only when SubTotal >= 5000
        /// </summary>
        public async Task<PurchaseSummary> GetPurchaseSummaryAsync(
            int customerId)
        {
            try
            {
                var cart = await GetCartByCustomerIdAsync(customerId);

                if (cart == null || !cart.CartItems.Any())
                    return new PurchaseSummary();

                var items = cart.CartItems.ToList();

                // Calculate totals
                var subTotal = items.Sum(ci => ci.LineTotal);
                var originalTotal = items.Sum(ci => ci.OriginalLineTotal);
                var productDiscountSaving = originalTotal - subTotal;

                // Cart level discount — only if SubTotal >= threshold
                bool cartDiscountApplied = subTotal >= _minimumAmountForDiscount;

                decimal cartDiscountAmount = cartDiscountApplied
                    ? Math.Round(subTotal * _cartDiscountPercentage / 100, 2)
                    : 0;

                decimal finalTotal = subTotal - cartDiscountAmount;
                decimal totalSaving = productDiscountSaving + cartDiscountAmount;

                // Message shown to customer
                string discountMessage = cartDiscountApplied
                    ? $"🎉 {_cartDiscountPercentage}% cart discount applied!"
                    : $"Add ₹{(_minimumAmountForDiscount - subTotal):N2} more to unlock {_cartDiscountPercentage}% cart discount!";

                return new PurchaseSummary
                {
                    Items = items,
                    SubTotal = subTotal,
                    OriginalTotal = originalTotal,
                    ProductDiscountSaving = productDiscountSaving,
                    CartDiscountPercentage = _cartDiscountPercentage,
                    DiscountAmount = cartDiscountAmount,
                    FinalTotal = finalTotal,
                    TotalSaving = totalSaving,
                    CartDiscountApplied = cartDiscountApplied,
                    DiscountMessage = discountMessage,
                    MinimumAmountForDiscount = _minimumAmountForDiscount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error getting purchase summary for customer {CustomerId}",
                    customerId);
                throw;
            }
        }
    }
}
