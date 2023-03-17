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
        [HttpPost("uploadProductImage"), DisableRequestSizeLimit]
        public List<string>? UploadProductImage([FromForm] UploadFile files)
        {
            try
            {
                var foldername = Path.Combine("Resources", "Images");
                var pathTosave = Path.Combine(Directory.GetCurrentDirectory(), foldername);
                List<string> imageUrls = new List<string>();
                foreach (var item in files.files)
                {
                    if (item.Length > 0)
                    {
                        var fileName = ContentDispositionHeaderValue.Parse(item.ContentDisposition).FileName.Trim().ToString();
                        var fullPath = Path.Combine(pathTosave, fileName);
                        var dbPath = Path.Combine(foldername, fileName);
                         using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            item.CopyTo(stream);
                            imageUrls.Add(fullPath);
                        }
                        _context.ProductImages.Add(new ProductImage() { ProductId = files.productId, ImageUrl = dbPath });

                    }
                }
                _context.SaveChanges();

                return imageUrls;
            }
            catch
            {
                return null;
            }
        }

        [Authorize]
        [HttpPost("uploadProductDocument"), DisableRequestSizeLimit]

        public List<string> UploadProductDocument([FromForm] UploadFile files)
        {
            try
            {
                var foldername = Path.Combine("Resources", "Documents");
                var pathTosave = Path.Combine(Directory.GetCurrentDirectory(), foldername);
                List<string> documentUrls = new List<string>();
                foreach (var item in files.files)
                {
                    if (item.Length > 0)
                    {
                        var fileName = ContentDispositionHeaderValue.Parse(item.ContentDisposition).FileName.Trim().ToString();
                        var fullPath = Path.Combine(pathTosave, fileName);
                        var dbPath = Path.Combine(foldername, fileName);
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            item.CopyTo(stream);
                            documentUrls.Add(fullPath);
                        }
                        _context.ProductDocuments.Add(new ProductDocument() { ProductId = files.productId, DocumentUrl = dbPath });

                    }
                }
                _context.SaveChanges();

                return documentUrls;
            }
            catch
            {
                return null;
            }
        }

    }
}
