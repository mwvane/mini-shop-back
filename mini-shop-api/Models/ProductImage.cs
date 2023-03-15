using System.ComponentModel.DataAnnotations;

namespace mini_shop_api.Models
{
    public class ProductImage {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public string ImageUrl { get; set; }
    }
}
