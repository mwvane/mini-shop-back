using mini_shop_api.Models;

namespace mini_shop_api.Controllers
{
    public class CartItem
    {
        public int Id { get; set; }
        public User User { get; set; }
        public Item Item { get; set; }
        public int Quantity { get; set; }   
    }
}
