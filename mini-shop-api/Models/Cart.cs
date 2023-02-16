using System.ComponentModel.DataAnnotations;

namespace mini_shop_api.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        [Required]
        public int ItemId { get; set; }
        [Required]
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }
        public int VoucherPrice { get; set; } = 0;
    }
}
