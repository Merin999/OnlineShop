using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShopping.Models
{
    public class Product : BaseEntity
    {
        public int ProductId { get; set; }
        [Required]
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPercentage { get; set; }
        public string Category { get; set; }

        public bool IsActive { get; set; } = true;
        public int StockQuantity { get; set; }

        [NotMapped]
        public decimal DiscountedPrice => Price - (Price * DiscountPercentage / 100);

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }
}
