using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShopping.Models
{
    public class Order : BaseEntity
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(18,2)")]
        public decimal OriginalTotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal CartDiscountPercentage { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CartDiscountAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal FinalTotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalSaving { get; set; }

        public string Status { get; set; } = "Placed";
    }
}
