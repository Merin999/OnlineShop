using OnlineShopping.Models;

namespace OnlineShopping.ViewModels
{
    public class CartViewModel
    {
        public PurchaseSummary Summary { get; set; } = new();
        public int CustomerId { get; set; }
    }
}
