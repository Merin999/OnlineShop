namespace OnlineShopping.Models
{
    public class PurchaseSummary
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public decimal SubTotal { get; set; }
        public decimal OriginalTotal { get; set; }
        public decimal ProductDiscountSaving { get; set; }
        public decimal CartDiscountPercentage { get; set; }  // ← Add this
        public decimal DiscountAmount { get; set; }
        public decimal FinalTotal { get; set; }
        public decimal TotalSaving { get; set; }
        public bool CartDiscountApplied { get; set; }
        public string DiscountMessage { get; set; } = string.Empty;
        public decimal MinimumAmountForDiscount { get; set; }
    }
}