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
        [HttpGet("getItems")]
        public List<Item> GetItems()
        {
            return _context.Items.ToList();
        }
    }
}
