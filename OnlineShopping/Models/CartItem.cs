using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShopping.Models
{
    public class CartItem : BaseEntity
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal OriginalPrice { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal DiscountPercentage { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

       
        [NotMapped]
        public decimal LineTotal => UnitPrice * Quantity;

        [NotMapped]
        public decimal OriginalLineTotal => OriginalPrice * Quantity;

        public Cart Cart { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
