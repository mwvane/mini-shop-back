namespace mini_shop_api.Models
{
    public class UploadFile
    {
        public int productId { get; set; }
        public IList<IFormFile> files { get; set; }
    }
}
