﻿using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        [HttpGet("getAllVouchers")]
        public List<Voucher> GetAllVouchers()
        {
            return this._context.Vouchers.ToList();
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
                    voucher.CreatedBy = Convert.ToInt32(loggeduserId.Value);
                    voucher.IsValid = voucher.ValidDate.Day - DateTime.Now.Day > 0 ? true : false;
                    _context.Vouchers.Add(voucher);
                    _context.SaveChanges();
                    return new Result() { Res = voucher.Id };
                }
            }
        }


        [Authorize(Roles = "admin,seller")]
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
                    voucher.IsValid = voucher.ValidDate.Day - DateTime.Now.Day > 0 ? true : false;
                    _context.Vouchers.Update(voucher);
                    _context.SaveChanges();
                    return new Result() { Res = true };
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
                    if(voucher != null)
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
