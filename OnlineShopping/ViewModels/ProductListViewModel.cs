using OnlineShopping.Models;

namespace OnlineShopping.ViewModels
{
    public class ProductListViewModel
    {
        public IEnumerable<Product> Products { get; set; }
            = new List<Product>();
        public IEnumerable<string> Categories { get; set; }
            = new List<string>();
        public string? SelectedCategory { get; set; }
        public int CartCount { get; set; }
    }
}
