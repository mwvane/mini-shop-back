using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using mini_shop_api.Models;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
namespace mini_shop_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        // readonly MyDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly User _user;
        public AdminController(IConfiguration config)
        {
            //_context = context;
            _configuration = config;

        }

        [HttpPost("removeItem")]
        public Result RemoveItem([FromBody] int[] itemIds)
        {

            Claim? loggeduserId = User.Claims.FirstOrDefault(x => x.Type.ToString().Equals("id", StringComparison.InvariantCultureIgnoreCase));
            List<Product> items = new List<Product>();
            if (loggeduserId == null)
            {
                return new Result() { Errors = new List<string> { "შეცდომაა, სცადე თავიდან!" } };
            }

            try
            {
                foreach (var id in itemIds)
                {
                    Product item = DbHelpers.GetItemById(id, this._configuration);
                    if (item != null)
                    {
                        items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                return new Result() { Errors = new List<string> { "პროდუქტი ვერ მოიძებნა" } };
            }

            User user = DbHelpers.GetUserById(Convert.ToInt32(loggeduserId.Value), this._configuration);
            try
            {
                foreach (var item in items)
                {
                    if (!UserAutorizationHelper.CanDeleteItem(user, item))
                    {
                        return new Result() { Errors = new List<string> { "თქვენ არ გაქვთ პროდუქტის შექმნის უფლება" } };
                    }
                    int? result = DbHelpers.CRUD($"Delete from Products where id = @0", new List<object> { item.Id }, _configuration);
                    if (result != null)
                    {
                        List<object?[]> productIds = DbHelpers.SelectMultiple($"select id from CartItems where ProductId = {item.Id}", _configuration);
                        if (productIds.Count() > 0)
                        {
                            foreach (var _idList in productIds)
                            {
                                DbHelpers.CRUD($"Delete from CartItems where id = @0", new List<object> { _idList[0] }, _configuration);
                            }
                        }
                    }
                }
                return new Result() { Res = true };
            }
            catch
            {
                return new Result() { Res = false, Errors = new List<string>() { "შეცდომაა" } };

            }
        }



        [HttpPost("updateItem")]
        public Result UpdateItem([FromBody] Product newItem)
        {
            Claim? id = User.Claims.FirstOrDefault(x => x.Type.ToString().Equals("id", StringComparison.InvariantCultureIgnoreCase));
            try
            {
                if (id == null)
                {
                    return new Result() { Errors = new List<string> { "შეცდომაა, სცადე თავიდან!" } };

                }
                Product item;
                try
                {
                    item = DbHelpers.GetItemById(newItem.Id, this._configuration);
                }
                catch (Exception ex)
                {
                    return new Result() { Errors = new List<string> { "პროდუქტი ვერ მოიძებნა" } };
                }

                User user = DbHelpers.GetUserById(Convert.ToInt32(id.Value), this._configuration);
                if (!UserAutorizationHelper.CanUpdateItem(user, item))
                {
                    return new Result() { Errors = new List<string> { "თქვენ არ გაქვთ პროდუქტის განახლების უფლება" } };
                }

                int? isUpdated = DbHelpers.CRUD(
                    $"Update Products Set Name = @0 ,Quantity = @1, Price = @2 where id = @3", new List<object> { newItem.Name, newItem.Quantity, newItem.Price, newItem.Id }, _configuration);
                return new Result() { Res = true };

            }
            catch
            {
                return new Result() { Errors = new List<string>() { "შეცდომაა" } };
            }

        }



        [HttpPost("createItem")]
        public Result CreateItem([FromBody] Product newItem)
        {
            Claim? id = User.Claims.FirstOrDefault(x => x.Type.ToString().Equals("id", StringComparison.InvariantCultureIgnoreCase));
            if (id == null)
            {
                return new Result() { Errors = new List<string> { "შეცდომაა, სცადეთ თავიდან!" } };

            }
            newItem.CreatedBy = Convert.ToInt32(id.Value);
            User user = DbHelpers.GetUserById(Convert.ToInt32(id.Value), this._configuration);
            if (!UserAutorizationHelper.CanCreateItem(user))
            {
                return new Result() { Errors = new List<string> { "თქვენ არ გაქვთ პროდუქტის წაშლის უფლება" } };
            }

            int? productId = DbHelpers.CRUD($"Insert into Products(Name,Quantity,Price,CreatedBy)values(@0,@1,@2,@3); SELECT SCOPE_IDENTITY()", new List<object> { newItem.Name, newItem.Quantity, newItem.Price, newItem.CreatedBy }, _configuration);
            if (productId != null)
            {
                return new Result() { Res = productId };
            }
            return new Result() { Errors = new List<string> { "პროდუქტი ვერ მოიძებნა" } };
        }
    }
}
