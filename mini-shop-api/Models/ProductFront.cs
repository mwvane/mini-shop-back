using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace mini_shop_api.Models
{
    public class ProductFront
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int CreatedBy { get; set; }
        public List<string> ImageUrls { get; set; }
        public List<string> DocumentUrls { get; set; }

    }
}
