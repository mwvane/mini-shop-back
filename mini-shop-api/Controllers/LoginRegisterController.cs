using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using mini_shop_api.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace mini_shop_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginRegisterController : ControllerBase
    {
        private readonly MyDbContext _context;
        public LoginRegisterController(MyDbContext context)
        {
            _context = context;
        }
        [HttpPost("login")]
        public Result Login([FromBody] Dictionary<string, string> payload)
        {
            string username = payload["username"];
            string password = payload["password"];
            if (username == null || password == null)
            {
                return new Result() { Errors = new List<string> { "Username and password is required!" } };
            }
            else
            {
                var user = _context.Users.Where(user => user.Email == username && user.Password == password).FirstOrDefault();

                if (user != null)
                {
                    user.Token = CreateJwt(user);
                    return new Result()
                    {
                        Res = new JwtAuthResponse
                        {
                            Token = user.Token,
                        }
                    };
                }
                return new Result() { Errors = new List<string> { "Invalid username or password" } };
            }
        }

        [HttpPost("signup")]
        public Result Signup([FromBody] Dictionary<string, string> payload)
        {
            bool usernameAlreadyExists = false;

            usernameAlreadyExists = _context.Users.Where(u => u.Email == payload["email"]).FirstOrDefault() != null;

            if (!usernameAlreadyExists)
            {
                User newUser = new()
                {
                    Email = payload["email"],
                    Firstname = payload["firstname"],
                    Lastname = payload["lastname"],
                    Password = payload["password"],
                    LastUpdated = DateTime.Now
                };
                List<string> errors = Validateuser(newUser);
                if (errors.Count == 0)
                {
                    _context.Add(newUser);
                    try
                    {
                        _context.SaveChanges();
                        return new Result() { Res = true };
                    }
                    catch (Exception exp)
                    {
                        Console.WriteLine(exp.Message);
                        return new Result() { Errors = errors };
                    }
                }
                return new Result() { Errors = errors };
            }
            return new Result() { Errors = new List<string>() { "User already exists!" } };
        }
        private List<string> Validateuser(User user)
        {
            List<string> errors = new List<string>();
            if (user.Firstname.Length < 2)
            {
                errors.Add("Firstname minimum length must be 2 character");
            }
            if (user.Password.Length < 6)
            {
                errors.Add("Password minimum length must be 6 symbols");
            }
            if (IsEmailValid(user.Email))
            {
                errors.Add("Incorrect email format");

            }
            return errors;
        }
        private bool IsEmailValid(string email)
        {
            return Regex.IsMatch(email, "^[a-zA-Z0-9_.+-]+@[email]+\\.[a-zA-Z0-9-.]+$", RegexOptions.IgnoreCase) == true;
        }

        private string CreateJwt(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Constants.JWT_SECURITY_KEY);
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim("id", user.Id.ToString()),
                new Claim(ClaimTypes.Role,user.Role),
                new Claim("lastname",$"{user.Lastname}"),
                new Claim("firstname",$"{user.Firstname}"),
                new Claim(ClaimTypes.Email,$"{user.Email}"),
            }); ;
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddDays(Constants.JWT_TOKEN_VALIDITY_MINS),
                SigningCredentials = credentials,
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }
    }
}
