using System.ComponentModel.DataAnnotations;

namespace mini_shop_api.Models
{
    public class SoldProduct
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public string ProductName { get; set; }
        [Required]
        public int UserId { get; set; }
        public int Quantity { get; set; }
        [Required]
        public double totalPrice { get; set; }
        [Required]
        public int VoucherPrice { get; set; }
    }
}
