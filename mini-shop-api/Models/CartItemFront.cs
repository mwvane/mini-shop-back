using System.Diagnostics.CodeAnalysis;

namespace mini_shop_api.Models
{
    public class CartItemFront
    {
        public int Id { get; set; }
        public User? User { get; set; }
        public Product? Product { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }
        public Voucher? Voucher { get; set; }
        public double? VoucherAmount { get; set; }

    }
}
