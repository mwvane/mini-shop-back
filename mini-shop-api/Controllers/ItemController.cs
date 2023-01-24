using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using mini_shop_api.Models;

namespace mini_shop_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly MyDbContext _context;
        public ItemController(MyDbContext context)
        {
            _context = context;
        }
        [HttpGet("getAllItems")]
        public List<Item> GetAllItems()
        {
            return _context.Items.ToList();
        }
        [HttpGet("getCartItems")]
        public List<CartItem> GetCartItems()
        {
            List<Cart> cart = _context.Cart.Where(c => c.UserId == 4).ToList();
            List<CartItem> cartItems = new List<CartItem>();
            if (cart.Count > 0)
            {
                foreach (var cartItem in cart)
                {
                    Item i = _context.Items.Where(item => item.Id == cartItem.ItemId).FirstOrDefault();
                    User u = _context.Users.Where(user => user.Id == cartItem.UserId).FirstOrDefault();
                    int quantity = cartItem.Quantitiy;
                    cartItems.Add(new CartItem() { Id = cartItem.Id, Item = i, Quantity = quantity, User = u });
                }
            }
            return cartItems;
        }
    }
}
