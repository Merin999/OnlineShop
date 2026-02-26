using Microsoft.AspNetCore.Mvc;
using OnlineShopping.Services;
using OnlineShopping.ViewModels;

namespace OnlineShopping.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly ILogger<ProductController> _logger;

        // Hardcoded CustomerId for now
        private const int DefaultCustomerId = 1;
        public ProductController(
            IProductService productService,
            ICartService cartService,
            ILogger<ProductController> logger)
        {
            _productService = productService;
            _cartService = cartService;
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string? category)
        {
            try
            {
                var products = string.IsNullOrEmpty(category)
                    ? await _productService.GetAllProductsAsync()
                    : await _productService.GetProductsByCategoryAsync(category);
                var viewModel = new ProductListViewModel
                {
                    Products = products,
                    Categories = await _productService.GetCategoriesAsync(),
                    SelectedCategory = category,
                    CartCount = await _cartService
                        .GetCartItemCountAsync(DefaultCustomerId)
                };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading product listing");
                TempData["Error"] = "Unable to load products. Please try again.";
                return View(new ProductListViewModel());
            }
        }
    }
}
