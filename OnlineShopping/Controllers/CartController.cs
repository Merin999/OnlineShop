using Microsoft.AspNetCore.Mvc;
using OnlineShopping.Services;
using OnlineShopping.ViewModels;

namespace OnlineShopping.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IProductService _productService;
        private readonly ILogger<CartController> _logger;

        // Hardcoded for now
        private const int DefaultCustomerId = 1;

        public CartController(
            ICartService cartService,
            IProductService productService,
            ILogger<CartController> logger)
        {
            _cartService = cartService;
            _productService = productService;
            _logger = logger;
        }
        
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var summary = await _cartService
                    .GetPurchaseSummaryAsync(DefaultCustomerId);

                var viewModel = new CartViewModel
                {
                    Summary = summary,
                    CustomerId = DefaultCustomerId
                };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading cart");
                TempData["Error"] = "Unable to load cart. Please try again.";
                return RedirectToAction("Index", "Product");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId,
            string? returnUrl)
        {
            try
            {
                await _cartService.AddToCartAsync(
                    DefaultCustomerId, productId);
                TempData["Success"] = "Product added to cart!";
                var summary = await _cartService
                    .GetPurchaseSummaryAsync(DefaultCustomerId);
                if (summary.CartDiscountApplied)
                    TempData["DiscountUnlocked"] =
                        $"🎉 {summary.CartDiscountPercentage}% cart discount unlocked!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error adding product {ProductId} to cart", productId);
                TempData["Error"] = ex.Message;
            }
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "Product");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            try
            {
                await _cartService.RemoveFromCartAsync(
                    DefaultCustomerId, cartItemId);
                TempData["Success"] = "Product removed from cart.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error removing cart item {CartItemId}", cartItemId);
                TempData["Error"] = "Error removing product. Please try again.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(
            int cartItemId, int quantity)
        {
            try
            {
                await _cartService.UpdateQuantityAsync(
                    DefaultCustomerId, cartItemId, quantity);

                TempData["Success"] = "Cart updated.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error updating quantity for cart item {CartItemId}",
                    cartItemId);
                TempData["Error"] = "Error updating cart. Please try again.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                await _cartService.ClearCartAsync(DefaultCustomerId);
                TempData["Success"] = "Cart cleared.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart");
                TempData["Error"] = "Error clearing cart. Please try again.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Summary()
        {
            try
            {
                var summary = await _cartService
                    .GetPurchaseSummaryAsync(DefaultCustomerId);
                if (!summary.Items.Any())
                {
                    TempData["Error"] = "Your cart is empty.";
                    return RedirectToAction(nameof(Index));
                }
                return View(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading summary");
                TempData["Error"] = "Error loading summary. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder()
        {
            try
            {
                var summary = await _cartService
                    .GetPurchaseSummaryAsync(DefaultCustomerId);
                if (!summary.Items.Any())
                {
                    TempData["Error"] = "Your cart is empty.";
                    return RedirectToAction(nameof(Index));
                }
                await _cartService.ClearCartAsync(DefaultCustomerId);
                TempData["OrderSuccess"] =
                    $"Order placed successfully! You saved ₹{summary.TotalSaving:N2}";
                return RedirectToAction("Index", "Product");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error placing order");
                TempData["Error"] = "Error placing order. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
