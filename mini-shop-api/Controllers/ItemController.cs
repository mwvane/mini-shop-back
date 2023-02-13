using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        [HttpGet("getAllItems")]
        public List<Item> GetAllItems()
        {
            return _context.Items.ToList();
        }

        [Authorize]
        [HttpGet("getAllUsers")]
        public List<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        [Authorize]
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


        [Authorize]
        [HttpGet("getItem")]
        public Item GetItem(int id)
        {
            return _context.Items.Where(item => item.Id == id).FirstOrDefault();
        }

        [HttpGet("getCartCount")]
        public int GetCartCount(int id)
        {
            return GetCartItems(id).Count;
        }
        [Authorize]
        [HttpPost("updateCartItemQuantity")]
        public Result UpdateCartItemQuantity([FromBody] Dictionary<string, int> payload)
        {
            int cartId = payload["id"];
            int quantity = payload["quantity"];
            try
            {
                Cart c = _context.Cart.Where(cartItem => cartItem.Id == cartId).FirstOrDefault();
                if (c != null)
                {
                    Item currentItem = GetItem(c.ItemId);
                    if (currentItem != null)
                    {
                        if (currentItem.Quantity > 0 || quantity < 0)
                        {
                            c.Quantitiy += quantity;
                            currentItem.Quantity += quantity * -1;
                            _context.SaveChanges();
                            return new Result() { Res = currentItem.Quantity };
                        }
                    }
                    return new Result() { Errors = new List<string>() { "პრდუქტის მარაგი ამოიწურა" } };
                }
                return new Result() { Errors = new List<string>() { "პროდუქტი ვერ მოიძებნა" } };
            }
            catch
            {
                return new Result() { Errors = new List<string>() { "შეცდომაა" } };
            }
        }

        [Authorize]
        [HttpPost("deleteCartItem")]
        public bool DeleteCartItem([FromBody] int itemId)
        {
            Cart c = _context.Cart.Where(item => item.Id == itemId).FirstOrDefault();
            Item item = GetItem(c.ItemId);
            if (c != null)
            {
                item.Quantity += c.Quantitiy;
                _context.Cart.Remove(c);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        [Authorize]
        [HttpPost("addToCart")]
        public Result AddToCart([FromBody] Dictionary<string, int> payload)
        {
            int itemId = payload["itemId"];
            int userId = payload["userId"];
            int quantity = payload["quantity"];
            var isExist = _context.Cart.Where(item => item.UserId == userId && item.ItemId == itemId).FirstOrDefault();
            if (isExist != null)
            {
                return new Result() { Errors = new List<string> { "პროდუქტი უკვე არსებობს კალათაში" } };
            }
            else
            {
                if (quantity > GetItem(itemId).Quantity)
                {
                    return new Result() { Errors = new List<string> { "მარაგი ამოიწურა!" } };
                }
                else
                {
                    Cart newItem = new Cart { ItemId = itemId, UserId = userId, Quantitiy = quantity };
                    _context.Cart.Add(newItem);
                    _context.SaveChanges();
                    UpdateCartItemQuantity(new Dictionary<string, int>() { { "id", newItem.Id }, { "quantity", 1 } });
                    return new Result() { Res = newItem.Id };
                }

            }
        }
    }
}
