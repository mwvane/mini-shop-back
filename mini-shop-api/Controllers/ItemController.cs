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
            foreach (var item in cart)
            {
                CartItem cartItem = new CartItem()
                {
                    Id = item.Id,
                    User = _context.Users.Where(val => val.Id == item.UserId).FirstOrDefault(),
                    Item = _context.Items.Where(val => val.Id == item.ItemId).FirstOrDefault(),
                    Quantity = item.Quantity,
                    TotalPrice = item.TotalPrice,
                    voucherPrice = item.VoucherPrice,
                };
                cartItems.Add(cartItem);
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
        [HttpPost("updateCartItem")]
        public Result UpdateCartItem([FromBody] CartItem cartItem)
        {
            Cart item = _context.Cart.Where(val => val.Id == cartItem.Id).FirstOrDefault();
            if (item == null)
            {
                return new Result() { Errors = new List<string>() { "პროდუქტი ვერ მოიძებნა" } };
            }
            item.VoucherPrice = cartItem.voucherPrice;
            item.Quantity = cartItem.Quantity;
            _context.SaveChanges();
            return new Result() { Res = true };
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
                            c.Quantity += quantity;
                            double totalPrice = Convert.ToDouble(c.Quantity * currentItem.Price - c.VoucherPrice);
                            if (totalPrice > 0)
                            {
                                c.TotalPrice = totalPrice;
                            }
                            else
                            {
                                c.TotalPrice = 0;
                            }
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
                item.Quantity += c.Quantity;
                _context.Cart.Remove(c);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        [Authorize]
        [HttpPost("addToCart")]
        public Result AddToCart([FromBody] Cart payload)
        {
            var isExist = _context.Cart.Where(item => item.UserId == payload.UserId && item.ItemId == payload.ItemId).FirstOrDefault();
            if (isExist != null)
            {
                return new Result() { Errors = new List<string> { "პროდუქტი უკვე არსებობს კალათაში" } };
            }
            else
            {
                if (payload.Quantity > GetItem(payload.ItemId).Quantity)
                {
                    return new Result() { Errors = new List<string> { "მარაგი ამოიწურა!" } };
                }
                else
                {
                    var product = _context.Items.Where(item => item.Id == payload.ItemId).FirstOrDefault();
                    payload.TotalPrice = Convert.ToDouble(payload.Quantity * product.Price - payload.VoucherPrice);
                    _context.Cart.Add(payload);
                    _context.SaveChanges();
                    UpdateCartItemQuantity(new Dictionary<string, int>() { { "id", payload.Id }, { "quantity", 1 } });
                    CartItem cartItem = new CartItem()
                    {
                        Id = payload.Id,
                        User = _context.Users.Where(val => val.Id == payload.UserId).FirstOrDefault(),
                        Item = product,
                        Quantity = payload.Quantity,
                        TotalPrice = payload.TotalPrice,
                        voucherPrice = payload.VoucherPrice,
                    };
                    return new Result() { Res = cartItem };
                }

            }
        }

        [Authorize]
        [HttpPost("buyProduct")]
        public Result BuyProduct([FromBody] int id)
        {
            Cart cartItem = _context.Cart.Where(item => item.Id == id).FirstOrDefault();
            if (cartItem != null)
            {
                var item = _context.Items.Where(val => val.Id == cartItem.ItemId).FirstOrDefault();
                var soldProduct = new SoldProduct()
                {
                    ProductId = cartItem.ItemId,
                    ProductName = item.Name,
                    UserId = cartItem.UserId,
                    Quantity = cartItem.Quantity,
                    totalPrice = cartItem.TotalPrice,
                    VoucherPrice = cartItem.VoucherPrice,
                };
                _context.SoldProducts.Add(soldProduct);
                _context.Cart.Remove(cartItem);
                _context.SaveChanges();
                return new Result() { Res = soldProduct };
            }
            return new Result() { Errors = new List<string>() { "პროდუქტი ვერ მოიძებნა" } };
        }

        [Authorize]
        [HttpGet("getSoldProducts")]
        public Result GetSoldProducts(int userId)
        {
            List<SoldProduct> myProducts = new List<SoldProduct>();
            var products = _context.SoldProducts.ToList();
            foreach (var item in products)
            {
                object product = new object();
                if (item.UserId == userId)
                {
                    myProducts.Add(item);
                }
            };
            return new Result() { Res = myProducts };
        }
    }
}
