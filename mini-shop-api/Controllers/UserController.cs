using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using mini_shop_api.Models;

namespace mini_shop_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly MyDbContext _context;
        public UserController(MyDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpPost("deleteUser")]
        public Result DeleteUser([FromBody] int[] userIds)
        {
            try
            {
                foreach (var id in userIds)
                {
                    _context.Remove(_context.Users.Where(user => user.Id == id).FirstOrDefault());
                }
                _context.SaveChanges();
                return new Result() { Res = true };
            }
            catch
            {
                return new Result() { Res = false, Errors = new List<string>() { "მომხმარებელი ვერ წაიშალა" } };

            }
        }
        [Authorize]
        [HttpPost("updateUser")]
        public Result UpdateUser([FromBody] User newUser)
        {
            if(newUser != null)
            {
                if(newUser.Id != null)
                {
                    try
                    {
                        _context.Update(newUser);
                        _context.SaveChanges();
                        return new Result() { Res = true };
                    }
                    catch
                    {
                        return new Result() { Res = false, Errors = new List<string>() { "შეცდომაა" } };
                    }
                }
                return new Result() { Res = false, Errors = new List<string>() { "მომხმარებელი ვერ მოიძებნა" } };
            }
            return new Result() { Res = false, Errors = new List<string>() { "მომხმარებელი ვერ მოიძებნა" } };

        }
    }
}
