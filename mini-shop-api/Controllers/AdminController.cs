using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using mini_shop_api.Models;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace mini_shop_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        // readonly MyDbContext _context;
        private readonly IConfiguration _configuration;
        public AdminController(IConfiguration config)
        {
            //_context = context;
            _configuration = config;
        }
        [HttpPost("removeItem")]
        public Result RemoveItem([FromBody] int itemId)
        {
            bool result = DbHelpers.CRUD($"Delete from Items where id = {itemId}", _configuration);
            if (result)
            {
                return new Result() { Res = true };
            }
            return new Result() { Errors = new List<string>() { "Item not deleted" } };

        }

        [HttpPost("updateItem")]
        public Result UpdateItem([FromBody] Item newItem)
        {
            bool isUpdated = DbHelpers.CRUD(
                $"Update Items Set Name = '{newItem.Name}',Quantity = {newItem.Quantity}, Price = {newItem.Price} where id = {newItem.Id}", _configuration);

            if (isUpdated)
            {
                return new Result() { Res = true };
            }
            return new Result() { Errors = new List<string>() { "Item not updated" } };
        }

        [HttpPost("createItem")]
        public Result CreateItem([FromBody] Item newItem)
        {
            bool isInserted = DbHelpers.CRUD($"Insert into Items(Name,Quantity,Price,CreatedBy)values('{newItem.Name}',{newItem.Quantity},{newItem.Price},{newItem.CreatedBy})", _configuration);
            if (isInserted)
            {
                return new Result() { Res = true };

            }
            return new Result() { Errors = new List<string> { "item not created" } };
        }
    }
}
