namespace OnlineShopping.Models
{
    public class Cart : BaseEntity
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }

        // Navigation
        public Customer Customer { get; set; } = null!;
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
