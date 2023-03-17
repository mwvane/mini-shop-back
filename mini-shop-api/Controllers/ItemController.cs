using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mini_shop_api.Helpers;
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
        [HttpGet("getAllProducts")]
        public List<ProductFront> GetAllProducts()
        {
            List<ProductFront> products = new List<ProductFront>();
            foreach (var item in _context.Products.ToList())
            {
                products.Add(Helper.ConvertProductForFront(item, _context));
            }
            return products;
        }

        [Authorize]
        [HttpGet("getAllUsers")]
        public List<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        [Authorize]
        [HttpGet("getCartItems")]
        public List<CartItemFront> GetCartItems(int id)

        {
            List<CartItem> cartItem = _context.CartItems.Where(c => c.UserId == id).ToList();
            List<CartItemFront> cartItemsFront = new List<CartItemFront>();
            foreach (var item in cartItem)
            {
                cartItemsFront.Add(Helper.ConvertCartItemForFront(item, _context));
            }
            return cartItemsFront;

        }


        [Authorize]
        [HttpGet("getProductById")]
        public Product GetProductById(int id)
        {
            return _context.Products.Where(item => item.Id == id).FirstOrDefault();
        }

        [Authorize]
        [HttpGet("getUserById")]
        public User GetUserById(int id)
        {
            return _context.Users.Where(item => item.Id == id).FirstOrDefault();
        }

        [HttpGet("getCartItemsCount")]
        public int GetCartItemsCount(int id)
        {
            return GetCartItems(id).Count;
        }

        [Authorize]
        [HttpPost("updateCartItem")]
        public Result UpdateCartItem([FromBody] CartItemFront cartItem)
        {
            CartItem item = _context.CartItems.Where(val => val.Id == cartItem.Id).FirstOrDefault();
            if (item == null)
            {
                return new Result() { Errors = new List<string>() { "პროდუქტი ვერ მოიძებნა" } };
            }
            var newCartItem = Helper.ConvertCartItemForBack(cartItem, _context);
            item.VoucherId = newCartItem.VoucherId;
            item.UserId = newCartItem.UserId;
            item.ProductId = newCartItem.ProductId;
            item.Quantity = newCartItem.Quantity;
            item.TotalPrice = newCartItem.TotalPrice;
            item.VoucherAmount = newCartItem.VoucherAmount;
            _context.CartItems.Update(item);
            _context.SaveChanges();
            return new Result() { Res = Helper.ConvertCartItemForFront(item, _context) };
        }

        [Authorize]
        [HttpPost("updateCartItemQuantity")]
        public Result UpdateCartItemQuantity([FromBody] Dictionary<string, int> payload)
        {
            int cartId = payload["id"];
            int quantity = payload["quantity"];
            try
            {
                CartItem c = _context.CartItems.Where(cartItem => cartItem.Id == cartId).FirstOrDefault();
                if (c != null)
                {
                    Product currentItem = GetProductById(c.ProductId);
                    if (currentItem != null)
                    {
                        if (currentItem.Quantity > 0 || quantity < 0)
                        {
                            c.Quantity += quantity;
                            double totalPrice = Convert.ToDouble(c.Quantity * currentItem.Price);
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
            var cartItem = _context.CartItems.Where(item => item.Id == itemId).FirstOrDefault();
            Product item = GetProductById(cartItem.ProductId);
            if (cartItem != null)
            {

                item.Quantity += cartItem.Quantity;
                if (cartItem.VoucherId != null)
                {
                    var voucher = _context.Vouchers.Where(item => item.Id == cartItem.VoucherId).FirstOrDefault();
                    if (voucher != null)
                    {
                        if (cartItem.VoucherAmount != null)
                        {
                            var cartItemFront = Helper.ConvertCartItemForFront(cartItem, _context);
                            if (cartItemFront != null && cartItemFront.Voucher.Status == "valid")
                            {
                                voucher.Price += Convert.ToInt32(cartItem.VoucherAmount);
                            }

                        }
                        if (voucher.ValidDate < DateTime.Now)
                        {
                            voucher.Status = "expired";
                        }
                        else if (voucher.Status == "preUsed")
                        {
                            voucher.Status = "valid";
                        }
                        _context.Vouchers.Update(voucher);

                    }

                }
                _context.CartItems.Remove(cartItem);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        [Authorize]
        [HttpPost("addToCart")]
        public Result AddToCart([FromBody] CartItem payload)
        {
            var cartItemFront = Helper.ConvertCartItemForFront(payload, _context);
            var isExist = _context.CartItems.Where(item => item.UserId == payload.UserId && item.ProductId == payload.ProductId).FirstOrDefault();
            if (isExist != null)
            {
                return new Result() { Errors = new List<string> { "პროდუქტი უკვე არსებობს კალათაში" } };
            }
            else
            {
                if (payload.Quantity > cartItemFront.Product.Quantity)
                {
                    return new Result() { Errors = new List<string> { "მარაგი ამოიწურა!" } };
                }
                else
                {
                    payload.TotalPrice = Convert.ToDouble(payload.Quantity * cartItemFront.Product.Price);
                    _context.CartItems.Add(payload);
                    _context.SaveChanges();
                    UpdateCartItemQuantity(new Dictionary<string, int>() { { "id", payload.Id }, { "quantity", 1 } });
                    return new Result() { Res = Helper.ConvertCartItemForFront(payload, _context) };
                }

            }
        }

        [Authorize]
        [HttpPost("buyProduct")]
        public Result BuyProduct([FromBody] int id)
        {
            CartItem cartItem = _context.CartItems.Where(item => item.Id == id).FirstOrDefault();
            if (cartItem != null)
            {
                var item = _context.Products.Where(val => val.Id == cartItem.ProductId).FirstOrDefault();
                if (item.Quantity < cartItem.Quantity)
                {
                    cartItem.Quantity = item.Quantity;
                    _context.CartItems.Update(cartItem);
                    _context.SaveChanges();
                    return new Result() { Errors = new List<string>() { $"საწყობში სულ {item.Quantity} პროდუქტია" } };
                }
                if (cartItem.VoucherId != null)
                {
                    var voucher = _context.Vouchers.Where(item => item.Id == cartItem.VoucherId).FirstOrDefault();
                    voucher.Status = "used";
                    _context.Vouchers.Update(voucher);
                    _context.SaveChanges();
                }
                var soldProduct = new Order()
                {
                    ProductId = cartItem.ProductId,
                    UserId = cartItem.UserId,
                    Quantity = cartItem.Quantity,
                    TotalPrice = cartItem.TotalPrice,
                    VoucherId = cartItem.VoucherId,
                    CreateDate = DateTime.Now,
                    VoucherAmount = Convert.ToInt32(cartItem.VoucherAmount),
                };
                _context.Orders.Add(soldProduct);
                _context.CartItems.Remove(cartItem);
                _context.SaveChanges();
                return new Result() { Res = Helper.ConvertOrderForFront(soldProduct, _context) };
            }
            return new Result() { Errors = new List<string>() { "პროდუქტი ვერ მოიძებნა" } };
        }

        [Authorize]
        [HttpGet("getOrders")]
        public Result GetOrders(int userId)
        {
            List<OrderFront> myProducts = new List<OrderFront>();
            var products = _context.Orders.ToList();
            foreach (var item in products)
            {
                if (item.UserId == userId)
                {
                    myProducts.Add(Helper.ConvertOrderForFront(item, _context));
                }
            };
            return new Result() { Res = myProducts };
        }
        [Authorize]
        [HttpGet("getAllOrders")]
        public Result GetAllOrders(int userId)
        {
            List<OrderFront> myProducts = new List<OrderFront>();
            var products = _context.Orders.ToList();
            foreach (var item in products)
            {
                myProducts.Add(Helper.ConvertOrderForFront(item, _context));
            }
            return new Result() { Res = myProducts };
        }
    }
}
