using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using mini_shop_api.Models;

namespace mini_shop_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly MyDbContext _context;
        public FileController(MyDbContext context)
        {
            _context= context;
        }

        [Authorize]
        [HttpPost("uplaodFile"), DisableRequestSizeLimit]
        public bool UploadProductImage([FromForm] UploadFile files)
        {
            try
            {
                var file = Request.Form.Files[0];
                var foldername = Path.Combine("Resources", "Images");
                var pathTosave = Path.Combine(Directory.GetCurrentDirectory(), foldername);
                if(file.Length> 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim().ToString();
                    var fullPath = Path.Combine(pathTosave, fileName);
                    var dbPath = Path.Combine(foldername, fileName);
                     using(var stream = new FileStream(fullPath,FileMode.Create))
                    {
                        file.CopyTo(stream);
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

    }
}
