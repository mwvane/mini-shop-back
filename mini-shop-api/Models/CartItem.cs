using Microsoft.AspNetCore.Routing.Constraints;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace mini_shop_api.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }

        [AllowNull]
        public double? VoucherAmount { get; set; } = 0;
        [AllowNull]
        public int? VoucherId { get; set; }
    }
}
