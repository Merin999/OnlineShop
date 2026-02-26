using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShopping.Models
{
    public class OrderItem : BaseEntity
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        [Column(TypeName = "decimal(18,2)")]
        public decimal OriginalPrice { get; set; }
        [Column(TypeName = "decimal(5,2)")]
        public decimal DiscountPercentage { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal LineTotal { get; set; }

        // Navigation
        public Order Order { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
