using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Models
{
    public class Customer : BaseEntity
    {
        public int CustomerId { get; set; }
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } 
        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; }
        [Required]
        [Phone]
        [StringLength(15)]
        public string Phone { get; set; } 
        [Required]
        [StringLength(200)]
        public string Address { get; set; }
        [Required]
        [StringLength(100)]
        public string City { get; set; } 

        [Required]
        [StringLength(100)]
        public string State { get; set; } 

        [Required]
        [StringLength(10)]
        public string PinCode { get; set; }
    }
}
