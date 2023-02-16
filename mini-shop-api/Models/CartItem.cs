namespace mini_shop_api.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public User User { get; set; }
        public Item Item { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice { get; set; } 
        public int voucherPrice { get; set; } = 0;
    }
}
