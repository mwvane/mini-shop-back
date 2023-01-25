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
        public List<CartItem> GetCartItems(int id)
        {
            List<Cart> cart = _context.Cart.Where(c => c.UserId == id).ToList();
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
        [HttpGet("getItemQuantity")]
        public int GetItemQuantity(int id)
        {
            return _context.Items.Where(item => item.Id == id).FirstOrDefault().Quantity;
        }

        [HttpGet("getCartCount")]
        public int GetCartCount(int id)
        {
            return GetCartItems(id).Count;
        }

        [HttpPost("updateCartItemQuantity")]
        public bool UpdateCartItemQuantity([FromBody] Dictionary<string, int> payload)
        {
            int cartId = payload["id"];
            int quantity = payload["quantity"];
            try
            {
                Cart c = _context.Cart.Where(cartItem => cartItem.Id == cartId).FirstOrDefault();
                if (c != null)
                {
                    c.Quantitiy = quantity;
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        [HttpPost("deleteCartItem")]
        public bool deleteCartItem([FromBody] Dictionary<string, int> payload)
        {
            int id = payload["id"];
            Cart c = _context.Cart.Where(item => item.Id == id).FirstOrDefault();
            if (c != null)
            {
                _context.Cart.Remove(c);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        [HttpPost("addToCart")]
        public Result AddToCart([FromBody] Dictionary<string, int> payload)
        {
            int itemId = payload["itemId"];
            int userId = payload["userId"];
            int quantity = payload["quantity"];
            var isExist = _context.Cart.Where(item => item.UserId == userId && item.ItemId == itemId).FirstOrDefault();
            if (isExist != null)
            {
                return new Result() { Errors = new List<string> { "Item already added in cart" } };
            }
            else
            {
                if (quantity > GetItemQuantity(itemId))
                {
                    return new Result() { Errors = new List<string> { "No enough items in the stok" } };
                }
                else
                {
                    _context.Cart.Add(new Cart() { ItemId = itemId, UserId = userId, Quantitiy = quantity });
                    _context.SaveChanges();
                    return new Result() { Res = true };
                }

            }
        }
    }
}
