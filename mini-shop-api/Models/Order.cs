using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace mini_shop_api.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int UserId { get; set; }
        public int Quantity { get; set; }
        [Required]
        public double TotalPrice { get; set; }
        [AllowNull]
        public int? VoucherId { get; set; }
        [AllowNull]
        public int? VoucherAmount { get; set; } = 0;
        [AllowNull]
        public DateTime? CreateDate { get; set; }

    }
}
