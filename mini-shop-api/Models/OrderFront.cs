using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace mini_shop_api.Models
{
    public class OrderFront
    {
        [Key]
        public int Id { get; set; }
        public Product? Product { get; set; }
        public User? User { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }
        public Voucher? Voucher { get; set; }
        public int? VoucherAmount { get; set; }
        public DateTime? CreateDate { get; set; }

    }
}
