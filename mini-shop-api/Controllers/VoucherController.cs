using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using mini_shop_api.Models;
using System.Security.Claims;

namespace mini_shop_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherController : ControllerBase
    {
        private readonly MyDbContext _context;
        public VoucherController(MyDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "admin,seller")]
        [HttpGet("getAllVouchers")]
        public List<Voucher> GetAllVouchers()
        {
            int userId = int.Parse(this.User.Claims.First(i => i.Type == "id").Value);
            var user = _context.Users.Where(item => item.Id == userId).FirstOrDefault();
            if (user != null)
            {
                if (user.Role == "admin")
                {
                    return this._context.Vouchers.ToList();
                }
                if (user.Role == "seller")
                {
                    var sellerVouchers = new List<Voucher>();
                    foreach (var item in _context.Vouchers.ToList())
                    {
                        if (item.CreatedBy == user.Id)
                        {
                            sellerVouchers.Add(item);
                        }
                    }
                    return sellerVouchers;
                }
            }
            return new List<Voucher>();
        }

        [Authorize]
        [HttpGet("getVoucherById")]
        public Voucher GetUserById(int id)
        {
            return _context.Vouchers.Where(item => item.Id == id).FirstOrDefault();
        }

        [Authorize]
        [HttpGet("getVoucher")]
        public Result GetVoucher(string key, int productId)
        {
            var product = this._context.Products.Where(item => item.Id == productId).FirstOrDefault();
            var voucher = this._context.Vouchers.Where(voucher => voucher.Key == key).FirstOrDefault();
            string? productCreatorRole = null;
            string? voucherCreatorRole = null;
            if (product != null && voucher != null)
            {
                var productCreator = _context.Users.Where(user => user.Id == product.CreatedBy).FirstOrDefault();
                if (productCreator != null)
                {
                    productCreatorRole = productCreator.Role;
                }
                var voucherCreator = _context.Users.Where(user => user.Id == voucher.CreatedBy).FirstOrDefault();
                if (voucherCreator != null)
                {
                    voucherCreatorRole = voucherCreator.Role;
                }

                if ((product.CreatedBy == voucher.CreatedBy && productCreatorRole == "seller") || voucherCreatorRole == "admin")
                {
                    if (voucher.ValidDate < DateTime.Now)
                    {
                        if (voucher.Status != "expired")
                        {
                            voucher.Status = "expired";
                            UpdateVoucher(voucher);
                        }
                        return new Result() { Errors = new List<string>() { "ვაუჩერი ვადაგასულია" } };
                    }
                    else if (voucher.Status == "used")
                    {
                        return new Result() { Errors = new List<string>() { "ვაუჩერი უკვე გამოყენებულია" } };

                    }
                    return new Result() { Res = voucher };
                }

                return new Result() { Errors = new List<string> { "აღნიშნულ პროდუქტზე , მითითებული ვაუჩერის გამოყენება შეუძლებელია!" } };
            }
            return new Result() { Errors = new List<string> { "პროდუქტი ვერ მოიძებნა!" } };
        }

        [Authorize(Roles = "admin,seller")]
        [HttpGet("generateVoucherKey")]
        public Result GenerateVoucherKey()
        {
            return new Result() { Res = Guid.NewGuid().ToString("N").Substring(0, 15) };
        }

        [Authorize(Roles = "admin,seller")]
        [HttpPost("createVoucher")]
        public Result CreateVoucher([FromBody] Voucher voucher)
        {
            if (voucher == null)
            {
                return new Result() { Errors = new List<string>() { "ვაუჩერის მონაცემები ცარიელია" } };
            }
            else
            {
                if (voucher.Key == null)
                {
                    return new Result() { Errors = new List<string>() { "ვაუჩერის კოდი ცარიელია" } };
                }
                else
                {
                    Claim? loggeduserId = User.Claims.FirstOrDefault(x => x.Type.ToString().Equals("id", StringComparison.InvariantCultureIgnoreCase));
                    if (voucher.ValidDate < DateTime.Now)
                    {
                        voucher.Status = "expired";
                    };
                    voucher.CreatedBy = Convert.ToInt32(loggeduserId.Value);
                    _context.Vouchers.Add(voucher);
                    _context.SaveChanges();
                    return new Result() { Res = voucher };
                }
            }
        }


        [Authorize]
        [HttpPost("updateVoucher")]
        public Result UpdateVoucher([FromBody] Voucher voucher)
        {
            if (voucher == null)
            {
                return new Result() { Errors = new List<string>() { "ვაუჩერი ვერ მოიძებნა" } };
            }
            else
            {
                if (voucher.Key == null)
                {
                    return new Result() { Errors = new List<string>() { "ვაუჩერის კოდი ცარიელია" } };
                }
                else
                {
                    if (voucher.ValidDate < DateTime.Now)
                    {
                        voucher.Status = "expired";
                    }
                    _context.Vouchers.Update(voucher);
                    _context.SaveChanges();
                    return new Result() { Res = voucher };
                }
            }
        }

        [Authorize(Roles = "admin,seller")]
        [HttpPost("deleteVoucher")]
        public Result DeleteVoucher([FromBody] int[] voucherIds)
        {
            if (voucherIds == null)
            {
                return new Result() { Errors = new List<string>() { "ვაუჩერი ვერ მოიძებნა" } };
            }
            else
            {
                foreach (var id in voucherIds)
                {
                    var voucher = _context.Vouchers.Where(voucher => voucher.Id == id).FirstOrDefault();
                    if (voucher != null)
                    {
                        _context.Vouchers.Remove(voucher);
                    }
                }
                _context.SaveChanges();
                return new Result() { Res = true };
            }
        }
    }
}
